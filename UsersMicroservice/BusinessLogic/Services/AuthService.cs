using AutoMapper;
using BusinessLogic.Contracts;
using BusinessLogic.Services.Interfaces;
using BusinessLogic.Validation.ErrorCodes;
using BusinessLogic.Validation.Messages;
using DataAccess.Entities;
using DataAccess.UnitOfWork.Interfaces;
using FluentValidation;
using Shared.Exceptions.ServerExceptions;
using ValidationException = Shared.Exceptions.ServerExceptions.ValidationException;

namespace BusinessLogic.Services
{
    public class AuthService : BaseService, IAuthService
    {
        private readonly IValidator<LoginDto> _loginValidator;
        private readonly IValidator<RegisterDto> _registerValidator;

        private readonly IPasswordHashingService _passwordHashingService;
        private readonly ITokenService _tokenService;

        public AuthService(IUnitOfWork unitOfWork,
            IMapper mapper,
            IValidator<LoginDto> loginValidator,
            IValidator<RegisterDto> registerValidator,
            IPasswordHashingService passwordHashingService,
            ITokenService tokenService)
            : base(unitOfWork, mapper)
        {
            _loginValidator = loginValidator;
            _registerValidator = registerValidator;
            _passwordHashingService = passwordHashingService;
            _tokenService = tokenService;
        }

        public async Task<(string jwtToken, string refreshToken)> RegisterAsync(RegisterDto userRegister, CancellationToken cancellationToken = default)
        {
            await _registerValidator.ValidateAndThrowAsync(userRegister, cancellationToken);

            if (!await IsUniqueEmail(userRegister.Email, cancellationToken))
                throw new ValidationException(
                    RegisterValidationErrorCodes.EmailIsNotUnique,
                    RegisterValidationMessages.EmailIsNotUnique,
                    nameof(userRegister.Email));

            var user = _mapper.Map<User>(userRegister);
            user.PasswordHash = _passwordHashingService.HashPassword(userRegister.Password);

            await _unitOfWork.InvokeWithTransactionAsync(async (token) =>
            {
                await _unitOfWork.UserRepository.CreateAsync(user, token);
            }, cancellationToken);

            var jwtToken = _tokenService.GenerateJwtToken(user.Id, user.Name, user.Email, user.Role);
            var refreshToken = _tokenService.GenerateRefreshToken();

            var refreshTokenModel = new RefreshToken
            {
                UserId = user.Id,
                Token = refreshToken.Token,
                Expires = refreshToken.Expires,
            };

            await UpsertRefreshTokenAsync(refreshTokenModel, cancellationToken);

            return new(jwtToken, refreshToken.Token);
        }

        public async Task<(string jwtToken, string refreshToken)> LoginAsync(LoginDto userLogin, CancellationToken cancellationToken = default)
        {
            await _loginValidator.ValidateAndThrowAsync(userLogin, cancellationToken);

            var user = await _unitOfWork.UserRepository.GetByEmailAsync(userLogin.Email, cancellationToken);
            var isValidCredentials = user != null && _passwordHashingService.VerifyPassword(userLogin.Password, user.PasswordHash);

            if (!isValidCredentials)
                throw new ValidationException(
                    errorCode: LoginValidationErrorCodes.EmailOrPasswordIsInvalid,
                    errorMessage: LoginValidationMessages.EmailOrPasswordIsInvalid);

            var jwtToken = _tokenService.GenerateJwtToken(user.Id, user.Name, user.Email, user.Role);
            var refreshToken = _tokenService.GenerateRefreshToken();

            var refreshTokenModel = new RefreshToken
            {
                UserId = user.Id,
                Token = refreshToken.Token,
                Expires = refreshToken.Expires,
            };

            await UpsertRefreshTokenAsync(refreshTokenModel, cancellationToken);

            return new(jwtToken, refreshToken.Token);
        }

        public async Task LogoutAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var refreshToken = await _unitOfWork.RefreshTokenRepository.GetByUserIdAsync(userId, cancellationToken);

            await _unitOfWork.RefreshTokenRepository.DeleteAsync(refreshToken, cancellationToken);
        }

        public async Task<(string jwtToken, string refreshToken)> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
        {
            var result = await ValidateRefreshTokenAsync(refreshToken, cancellationToken);
            if (!result.IsValid)
                throw new UnauthorizedException("Refresh token expired or invalid");

            var user = await _unitOfWork.UserRepository.GetByIdAsync(result.UserId, cancellationToken);
            if (user == null)
                throw new UnauthorizedException("Refresh token expired or invalid");

            var newRefreshToken = await RegenerateRefreshTokenValueAsync(user.Id, cancellationToken);
            var jwtToken = _tokenService.GenerateJwtToken(user.Id, user.Name, user.Email, user.Role);

            return new(jwtToken, newRefreshToken.Token);
        }

        public async Task<(bool IsValid, Guid UserId)> ValidateRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
        {
            var storedToken = await _unitOfWork.RefreshTokenRepository.GetByRefreshTokenAsync(refreshToken, cancellationToken);
            if (storedToken == null || storedToken.Token != refreshToken || storedToken.Expires < DateTime.UtcNow)
                return new(false, Guid.Empty);

            return new(true, storedToken.UserId);
        }

        private async Task<bool> IsUniqueEmail(string email, CancellationToken token = default)
        {
            return !await _unitOfWork.UserRepository.ContainsEmailAsync(email, token);
        }

        /// <summary>
        /// Updates an existing one, otherwise creates a new one
        /// </summary>
        /// <param name="refreshToken">Refresh token</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation if needed.</param>
        private async Task UpsertRefreshTokenAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default)
        {
            var existingRefreshToken = await _unitOfWork.RefreshTokenRepository.GetByUserIdAsync(refreshToken.UserId, cancellationToken);

            if (existingRefreshToken != null)
            {
                refreshToken.Id = existingRefreshToken.Id;

                await _unitOfWork.RefreshTokenRepository.UpdateAsync(refreshToken, cancellationToken);
            }
            else
            {
                await _unitOfWork.RefreshTokenRepository.CreateAsync(refreshToken, cancellationToken);
            }
        }

        /// <summary>
        /// Regenerates the value of the refresh token while preserving its original expiration time.
        /// </summary>
        /// <param name="userId">User id</param>
        /// <returns>New <see cref="RefreshToken"/> with the same expiration time as the previous one. If the old refresh token does not exist, returns <see langword="null"/></returns>
        private async Task<RefreshToken> RegenerateRefreshTokenValueAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var oldRefreshToken = await _unitOfWork.RefreshTokenRepository.GetByUserIdAsync(userId, cancellationToken);
            if (oldRefreshToken == null)
                return null;

            var newRefreshToken = _tokenService.GenerateRefreshToken();

            oldRefreshToken.Token = newRefreshToken.Token;

            await UpsertRefreshTokenAsync(oldRefreshToken, cancellationToken);

            return oldRefreshToken;
        }
    }
}
