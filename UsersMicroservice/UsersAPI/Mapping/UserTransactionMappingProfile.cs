using AutoMapper;
using BusinessLogic.Contracts;
using Shared.Grpc.User;

namespace UsersAPI.Mapping
{
    public class UserTransactionMappingProfile : Profile
    {
        public UserTransactionMappingProfile()
        {
            CreateMap<CreateUserTransactionDto, CreateTransactionRequest>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId.ToString()))
                .ForMember(dest => dest.EventId, opt => opt.MapFrom(src => src.EventId.ToString()));

            CreateMap<CreateTransactionRequest, CreateUserTransactionDto>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => Guid.Parse(src.UserId)))
                .ForMember(dest => dest.EventId, opt => opt.MapFrom(src => Guid.Parse(src.EventId)));
        }
    }
}
