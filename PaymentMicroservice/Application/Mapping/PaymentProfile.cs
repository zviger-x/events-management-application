using Application.Contracts;
using Application.MediatR.Commands;
using AutoMapper;

namespace Application.Mapping
{
    public class PaymentProfile : Profile
    {
        public PaymentProfile()
        {
            CreateMap<ProcessPaymentCommand, CreateUserTransactionDto>();
            CreateMap<CreateUserTransactionDto, ProcessPaymentCommand>();
        }
    }
}
