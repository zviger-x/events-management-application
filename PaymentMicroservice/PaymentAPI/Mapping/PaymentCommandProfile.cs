using Application.MediatR.Commands;
using AutoMapper;
using PaymentAPI.Services.Grpc.Payment;

namespace PaymentAPI.Mapping
{
    public class PaymentCommandProfile : Profile
    {
        protected PaymentCommandProfile()
        {
            CreateMap<ProcessPaymentRequest, ProcessPaymentCommand>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => Guid.Parse(src.UserId)))
                .ForMember(dest => dest.EventId, opt => opt.MapFrom(src => Guid.Parse(src.EventId)));
        }
    }
}
