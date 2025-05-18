using Application.Contracts;
using AutoMapper;
using Shared.Grpc.Payment;

namespace Infrastructure.Mapping
{
    public class MappingProfile : Profile
    {
        // TODO: Переименовать профиль!
        public MappingProfile()
        {
            CreateMap<ProcessPaymentDto, ProcessPaymentRequest>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId.ToString()))
                .ForMember(dest => dest.EventId, opt => opt.MapFrom(src => src.EventId.ToString()));

            CreateMap<ProcessPaymentRequest, ProcessPaymentDto>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => Guid.Parse(src.UserId)))
                .ForMember(dest => dest.EventId, opt => opt.MapFrom(src => Guid.Parse(src.EventId)));
        }
    }
}
