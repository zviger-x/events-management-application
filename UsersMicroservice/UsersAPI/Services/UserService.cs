using BusinessLogic.Services.Interfaces;
using Google.Protobuf.Collections;
using Grpc.Core;
using UsersAPI.Protos;

namespace UsersAPI.Services
{
     public class UserService : UserServiceGRPC.UserServiceGRPCBase
     {
        private protected IUserService _userService;

        public UserService(IUserService userService)
        {
            _userService = userService;
        }

        public override async Task<Response> CreateUser(CreateUserRequest request, ServerCallContext context)
        {
            var protoUser = request.User;
            var user = new DataAccess.Entities.User()
            {
                Name = protoUser.Name,
                Surname = protoUser.Surname,
                Email = protoUser.Email,
                Password = protoUser.Password,
                Role = protoUser.Role,
            };

            var response = await _userService.CreateAsync(user);

            var errorsMap = new MapField<string, StringArray>();
            foreach (var kvp in response.Errors)
            {
                var stringArray = new StringArray();
                stringArray.Values.AddRange(kvp.Value);
                errorsMap.Add(kvp.Key, stringArray);
            }

            var protoResponse = new Response() { HasErrors = response.HasErrors };
            foreach (var error in errorsMap)
                protoResponse.Errors.Add(error.Key, error.Value);

            return protoResponse;
        }

        // Реализация метода обновления пользователя (UpdateUser)
        public override async Task<Response> UpdateUser(UpdateUserRequest request, ServerCallContext context)
        {
            var protoUser = request.User;
            var user = new DataAccess.Entities.User()
            {
                Id = Guid.Parse(protoUser.Id),
                Name = protoUser.Name,
                Surname = protoUser.Surname,
                Email = protoUser.Email,
                Password = protoUser.Password,
                Role = protoUser.Role,
            };

            var response = await _userService.UpdateAsync(user);

            var errorsMap = new MapField<string, StringArray>();
            foreach (var kvp in response.Errors)
            {
                var stringArray = new StringArray();
                stringArray.Values.AddRange(kvp.Value);
                errorsMap.Add(kvp.Key, stringArray);
            }

            var protoResponse = new Response() { HasErrors = response.HasErrors };
            foreach (var error in errorsMap)
                protoResponse.Errors.Add(error.Key, error.Value);

            return protoResponse;
        }

        // Реализация метода удаления пользователя (DeleteUser)
        public override async Task<Response> DeleteUser(DeleteUserRequest request, ServerCallContext context)
        {
            var response = await _userService.DeleteAsync(Guid.Parse(request.Id));

            var errorsMap = new MapField<string, StringArray>();
            foreach (var kvp in response.Errors)
            {
                var stringArray = new StringArray();
                stringArray.Values.AddRange(kvp.Value);
                errorsMap.Add(kvp.Key, stringArray);
            }

            var protoResponse = new Response() { HasErrors = response.HasErrors };
            foreach (var error in errorsMap)
                protoResponse.Errors.Add(error.Key, error.Value);

            return protoResponse;
        }

        // Реализация метода получения пользователя по Id (GetUserById)
        public override async Task<ResponseUser> GetUserById(GetUserByIdRequest request, ServerCallContext context)
        {
            var response = await _userService.GetByIdAsync(Guid.Parse(request.Id));

            var errorsMap = new MapField<string, StringArray>();
            foreach (var kvp in response.Errors)
            {
                var stringArray = new StringArray();
                stringArray.Values.AddRange(kvp.Value);
                errorsMap.Add(kvp.Key, stringArray);
            }

            var baseProtoResponse = new Response() { HasErrors = response.HasErrors };
            foreach (var error in errorsMap)
                baseProtoResponse.Errors.Add(error.Key, error.Value);

            var user = response.DataTransferObject;
            var protoUser = new User()
            {
                Id = user.Id.ToString(),
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                Password = user.Password,
                Role = user.Role,
            };

            var protoResponse = new ResponseUser()
            {
                BaseResponse = baseProtoResponse,
                DataTransferObject = protoUser,
            };

            return protoResponse;
        }

        // Реализация метода получения всех пользователей (GetAllUsers)
        public override Task<ResponseUsers> GetAllUsers(GetAllUsersRequest request, ServerCallContext context)
        {
            var response = _userService.GetAll();

            var errorsMap = new MapField<string, StringArray>();
            foreach (var kvp in response.Errors)
            {
                var stringArray = new StringArray();
                stringArray.Values.AddRange(kvp.Value);
                errorsMap.Add(kvp.Key, stringArray);
            }

            var baseProtoResponse = new Response() { HasErrors = response.HasErrors };
            foreach (var error in errorsMap)
                baseProtoResponse.Errors.Add(error.Key, error.Value);

            var users = response.DataTransferObject;
            var protoResponse = new ResponseUsers()
            {
                BaseResponse = baseProtoResponse,
            };

            foreach (var user in users)
            {
                var protoUser = new User()
                {
                    Id = user.Id.ToString(),
                    Name = user.Name,
                    Surname = user.Surname,
                    Email = user.Email,
                    Password = user.Password,
                    Role = user.Role,
                };
                protoResponse.DataTransferObject.Add(protoUser);
            }

            return Task.FromResult(protoResponse);
        }
    }
}
