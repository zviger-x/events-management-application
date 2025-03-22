using AutoMapper;
using BusinessLogic.Contracts;
using BusinessLogic.Services.Interfaces;
using BusinessLogic.Validation.ErrorCodes;
using BusinessLogic.Validation.Messages;
using BusinessLogic.Validation.Validators.Interfaces;
using DataAccess.Entities;
using DataAccess.UnitOfWork.Interfaces;
using FluentValidation;
using FluentValidation.Results;

namespace BusinessLogic.Services
{
    public class AuthService : BaseService, IAuthService
    {
        private readonly ILoginDTOValidator _loginValidator;
        private readonly IRegisterDTOValidator _registerValidator;

        private readonly IPasswordHashingService _passwordHashingService;
        private readonly IJwtTokenService _jwtTokenService;

        public AuthService(IUnitOfWork unitOfWork,
            IMapper mapper,
            ILoginDTOValidator loginValidator,
            IRegisterDTOValidator registerValidator,
            IPasswordHashingService passwordHashingService,
            IJwtTokenService jwtTokenService)
            : base(unitOfWork, mapper)
        {
            _loginValidator = loginValidator;
            _registerValidator = registerValidator;
            _passwordHashingService = passwordHashingService;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<(string jwtToken, string refreshToken)> RegisterAsync(RegisterDTO userRegister, CancellationToken cancellationToken = default)
        {
            await _registerValidator.ValidateAndThrowAsync(userRegister, cancellationToken);

            var user = _mapper.Map<User>(userRegister);
            user.PasswordHash = _passwordHashingService.HashPassword(userRegister.Password);

            await _unitOfWork.InvokeWithTransactionAsync(async (token) =>
            {
                await _unitOfWork.UserRepository.CreateAsync(user, token);
            }, cancellationToken);

            var jwtToken = _jwtTokenService.GenerateToken(user.Id, user.Name, user.Email, user.Role);
            var refreshToken = _jwtTokenService.GenerateRefreshToken(user.Id);

            await _unitOfWork.RefreshTokenRepository.UpsertAsync(refreshToken, cancellationToken);
            await _unitOfWork.RefreshTokenRepository.SaveChangesAsync(cancellationToken);

            return new (jwtToken, refreshToken.Token);
        }

        public async Task<(string jwtToken, string refreshToken)> LoginAsync(LoginDTO userLogin, CancellationToken cancellationToken = default)
        {
            await _loginValidator.ValidateAndThrowAsync(userLogin, cancellationToken);

            var user = await _unitOfWork.UserRepository.GetByEmailAsync(userLogin.Email, cancellationToken);

            if (user == null || !_passwordHashingService.VerifyPassword(userLogin.Password, user.PasswordHash))
            {
                throw new ValidationException(new List<ValidationFailure>
                {
                    new ValidationFailure()
                    {
                        ErrorCode = LoginValidationErrorCodes.EmailOrPasswordIsInvalid,
                        ErrorMessage = LoginValidationMessages.EmailOrPasswordIsInvalid
                    }
                });
            }

            var jwtToken = _jwtTokenService.GenerateToken(user.Id, user.Name, user.Email, user.Role);
            var refreshToken = _jwtTokenService.GenerateRefreshToken(user.Id);

            await _unitOfWork.RefreshTokenRepository.UpsertAsync(refreshToken, cancellationToken);
            await _unitOfWork.RefreshTokenRepository.SaveChangesAsync(cancellationToken);

            return new(jwtToken, refreshToken.Token);
        }

        public async Task<string> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
        {
            var result = await _jwtTokenService.ValidateRefreshTokenAsync(refreshToken, cancellationToken);
            if (!result.IsValid)
                return null;

            var user = await _unitOfWork.UserRepository.GetByIdAsync(result.UserId, cancellationToken);
            var jwtToken = _jwtTokenService.GenerateToken(user.Id, user.Name, user.Email, user.Role);
            return jwtToken;
        }
    }
}
