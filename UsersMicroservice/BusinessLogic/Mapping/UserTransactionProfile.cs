using AutoMapper;
using BusinessLogic.Contracts;
using DataAccess.Entities;

namespace BusinessLogic.Mapping
{
    public class UserTransactionProfile : Profile
    {
        public UserTransactionProfile()
        {
            CreateMap<CreateUserTransactionDto, UserTransaction>();
            CreateMap<UserTransaction, CreateUserTransactionDto>();

            CreateMap<UpdateUserTransactionDto, UserTransaction>();
            CreateMap<UserTransaction, UpdateUserTransactionDto>();
        }
    }
}
