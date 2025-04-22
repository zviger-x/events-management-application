using AutoMapper;
using BusinessLogic.Contracts;
using BusinessLogic.Exceptions;
using BusinessLogic.Services.Interfaces;
using BusinessLogic.Validation.ErrorCodes;
using BusinessLogic.Validation.Messages;
using DataAccess.Entities;
using DataAccess.UnitOfWork.Interfaces;
using FluentValidation;
using ValidationException = BusinessLogic.Exceptions.ValidationException;

namespace BusinessLogic.Services
{
    public class AuthService : BaseService, IAuthService
    {
        private readonly IValidator<LoginDTO> _loginValidator;
        private readonly IValidator<RegisterDTO> _registerValidator;

        private readonly IPasswordHashingService _passwordHashingService;
        private readonly ITokenService _tokenService;

        private readonly ICurrentUserService _currentUserService;

        public AuthService(IUnitOfWork unitOfWork,
            IMapper mapper,
            IValidator<LoginDTO> loginValidator,
            IValidator<RegisterDTO> registerValidator,
            IPasswordHashingService passwordHashingService,
            ITokenService tokenService,
            ICurrentUserService currentUserService)
            : base(unitOfWork, mapper)
        {
            _loginValidator = loginValidator;
            _registerValidator = registerValidator;
            _passwordHashingService = passwordHashingService;
            _tokenService = tokenService;
            _currentUserService = currentUserService;
        }

        public async Task<(string jwtToken, string refreshToken)> RegisterAsync(RegisterDTO userRegister, CancellationToken cancellationToken = default)
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
            var refreshToken = _tokenService.GenerateRefreshToken(user.Id);

            await UpsertRefreshTokenAsync(refreshToken, cancellationToken);

            return new(jwtToken, refreshToken.Token);
        }

        public async Task<(string jwtToken, string refreshToken)> LoginAsync(LoginDTO userLogin, CancellationToken cancellationToken = default)
        {
            await _loginValidator.ValidateAndThrowAsync(userLogin, cancellationToken);

            var user = await _unitOfWork.UserRepository.GetByEmailAsync(userLogin.Email, cancellationToken);
            var isValidEmailOrPassword = user != null && _passwordHashingService.VerifyPassword(userLogin.Password, user.PasswordHash);

            if (!isValidEmailOrPassword)
                throw new ValidationException(
                    errorCode: LoginValidationErrorCodes.EmailOrPasswordIsInvalid,
                    errorMessage: LoginValidationMessages.EmailOrPasswordIsInvalid);

            var jwtToken = _tokenService.GenerateJwtToken(user.Id, user.Name, user.Email, user.Role);
            var refreshToken = _tokenService.GenerateRefreshToken(user.Id);

            await UpsertRefreshTokenAsync(refreshToken, cancellationToken);

            return new(jwtToken, refreshToken.Token);
        }

        public async Task LogoutAsync(CancellationToken cancellationToken = default)
        {
            var userId = _currentUserService.GetUserIdOrThrow();
            var refreshToken = await _unitOfWork.RefreshTokenRepository.GetByUserIdAsync(userId, cancellationToken);

            await _unitOfWork.RefreshTokenRepository.DeleteAsync(refreshToken, cancellationToken);
        }

        public async Task<string> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
        {
            var result = await _tokenService.ValidateRefreshTokenAsync(refreshToken, cancellationToken);
            if (!result.IsValid)
                throw new UnauthorizedException("Refresh token expired or invalid");

            var user = await _unitOfWork.UserRepository.GetByIdAsync(result.UserId, cancellationToken);
            if (user == null)
                throw new UnauthorizedException("Refresh token expired or invalid");

            var jwtToken = _tokenService.GenerateJwtToken(user.Id, user.Name, user.Email, user.Role);
            return jwtToken;
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
    }
}
