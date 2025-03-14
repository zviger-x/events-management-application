using DataAccess.Entities;
using FluentValidation;

namespace BusinessLogic.Validation.Validators.Interfaces
{
    public interface IUserTransactionValidator : IValidator<UserTransaction>
    {
    }
}
