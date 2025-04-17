using AutoMapper;
using BusinessLogic.Caching.Interfaces;
using BusinessLogic.Contracts;
using BusinessLogic.Exceptions;
using BusinessLogic.Services.Interfaces;
using BusinessLogic.Validation.ErrorCodes;
using BusinessLogic.Validation.Messages;
using BusinessLogic.Validation.Validators.Interfaces;
using DataAccess.Entities;
using DataAccess.UnitOfWork.Interfaces;
using FluentValidation;

namespace BusinessLogic.Services
{
    public class AuthService : BaseService, IAuthService
    {
        private readonly ILoginDTOValidator _loginValidator;
        private readonly IRegisterDTOValidator _registerValidator;

        private readonly IPasswordHashingService _passwordHashingService;
        private readonly ITokenService _jwtTokenService;

        private readonly ICacheService _cacheService;

        public AuthService(IUnitOfWork unitOfWork,
            IMapper mapper,
            ILoginDTOValidator loginValidator,
            IRegisterDTOValidator registerValidator,
            IPasswordHashingService passwordHashingService,
            ITokenService jwtTokenService,
            ICacheService cacheService)
            : base(unitOfWork, mapper)
        {
            _loginValidator = loginValidator;
            _registerValidator = registerValidator;
            _passwordHashingService = passwordHashingService;
            _jwtTokenService = jwtTokenService;
            _cacheService = cacheService;
        }

        public async Task<(string jwtToken, string refreshToken)> RegisterAsync(RegisterDTO userRegister, CancellationToken cancellationToken = default)
        {
            await _registerValidator.ValidateAndThrowAsync(userRegister, cancellationToken);

            if (!await IsUniqueEmail(userRegister.Email, cancellationToken))
                throw new ServiceValidationException(
                    RegisterValidationErrorCodes.EmailIsNotUnique,
                    RegisterValidationMessages.EmailIsNotUnique,
                    nameof(userRegister.Email));

            var user = _mapper.Map<User>(userRegister);
            user.PasswordHash = _passwordHashingService.HashPassword(userRegister.Password);

            await _unitOfWork.InvokeWithTransactionAsync(async (token) =>
            {
                await _unitOfWork.UserRepository.CreateAsync(user, token);
            }, cancellationToken);

            var jwtToken = _jwtTokenService.GenerateJwtToken(user.Id, user.Name, user.Email, user.Role);

            var refreshToken = _jwtTokenService.GenerateRefreshToken(user.Id);
            await UpsertRefreshTokenAsync(refreshToken, cancellationToken);

            return new(jwtToken, refreshToken.Token);
        }

        public async Task<(string jwtToken, string refreshToken)> LoginAsync(LoginDTO userLogin, CancellationToken cancellationToken = default)
        {
            await _loginValidator.ValidateAndThrowAsync(userLogin, cancellationToken);

            var user = await _unitOfWork.UserRepository.GetByEmailAsync(userLogin.Email, cancellationToken);

            if (user == null || !_passwordHashingService.VerifyPassword(userLogin.Password, user.PasswordHash))
                throw new ServiceValidationException(
                    LoginValidationErrorCodes.EmailOrPasswordIsInvalid,
                    LoginValidationMessages.EmailOrPasswordIsInvalid);

            var jwtToken = _jwtTokenService.GenerateJwtToken(user.Id, user.Name, user.Email, user.Role);

            var refreshToken = _jwtTokenService.GenerateRefreshToken(user.Id);
            await UpsertRefreshTokenAsync(refreshToken, cancellationToken);

            return new(jwtToken, refreshToken.Token);
        }

        public async Task LogoutAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var refreshToken = await _unitOfWork.RefreshTokenRepository.GetByUserIdAsync(id);
            await _unitOfWork.RefreshTokenRepository.DeleteAsync(refreshToken);
        }

        public async Task<string> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
        {
            var result = await _jwtTokenService.ValidateRefreshTokenAsync(refreshToken, cancellationToken);
            if (!result.IsValid)
                return null;

            var user = await _unitOfWork.UserRepository.GetByIdAsync(result.UserId, cancellationToken);
            if (user == null)
                return null;

            var jwtToken = _jwtTokenService.GenerateJwtToken(user.Id, user.Name, user.Email, user.Role);
            return jwtToken;
        }

        private async Task<bool> IsUniqueEmail(string email, CancellationToken token = default)
        {
            return !await _unitOfWork.UserRepository.ContainsEmailAsync(email);
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
