using Application.MediatR.Commands;
using AutoMapper;
using Shared.Kafka.Contracts.Payment;

namespace Application.Mapping
{
    public class PaymentConfirmedMessageProfile : Profile
    {
        public PaymentConfirmedMessageProfile()
        {
            CreateMap<ProcessPaymentCommand, PaymentConfirmedDto>()
                .ForMember(dest => dest.ConfirmedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.TargetUser, opt => opt.MapFrom(src => src.UserId));
        }
    }
}
