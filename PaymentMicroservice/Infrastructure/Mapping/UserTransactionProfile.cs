using Application.Contracts;
using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Infrastructure.Clients.Grpc.User;

namespace Infrastructure.Mapping
{
    internal class UserTransactionProfile : Profile
    {
        protected UserTransactionProfile()
        {
            // Guid to string, DateTime to Proto.TimeStamp
            CreateMap<CreateUserTransactionDto, CreateTransactionRequest>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId.ToString()))
                .ForMember(dest => dest.EventId, opt => opt.MapFrom(src => src.EventId.ToString()))
                .ForMember(dest => dest.TransactionDate, opt => opt.MapFrom(src => Timestamp.FromDateTime(src.TransactionDate)));

            // String to Guid, Proto.TimeStamp to DateTime
            CreateMap<CreateTransactionRequest, CreateUserTransactionDto>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => Guid.Parse(src.UserId)))
                .ForMember(dest => dest.EventId, opt => opt.MapFrom(src => Guid.Parse(src.EventId)))
                .ForMember(dest => dest.TransactionDate, opt => opt.MapFrom(src => src.TransactionDate.ToDateTime()));
        }
    }
}
