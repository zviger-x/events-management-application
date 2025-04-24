using Application.Contracts;
using Application.MediatR.Commands;
using AutoMapper;

namespace Application.Mapping
{
    internal class PaymentProfile : Profile
    {
        public PaymentProfile()
        {
            CreateMap<ProcessPaymentCommand, CreateUserTransactionDto>();
            CreateMap<CreateUserTransactionDto, ProcessPaymentCommand>();
        }
    }
}
