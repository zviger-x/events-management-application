using BusinessLogic.Contracts;
using BusinessLogic.Services.Interfaces;
using DataAccess.UnitOfWork.Interfaces;

namespace BusinessLogic.Services
{
    public class AuthService : BaseService, IAuthService
    {
        public AuthService(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
            #warning Сделать валидацию моделей
        }

        public Task LoginAsync(LoginDTO userLogin, CancellationToken cancellationToken)
        {
            #warning Сделать вход пользователя и возврат токена.
            return Task.FromException(new NotImplementedException());
        }

        public Task RegisterAsync(RegisterDTO userRegister, CancellationToken cancellationToken)
        {
            #warning Сделать регистрацию пользователя и возврат токена.
            return Task.FromException(new NotImplementedException());
        }
    }
}
