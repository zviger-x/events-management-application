using DataAccess.Entities.Interfaces;

namespace BusinessLogic.Validation.Validators.Interfaces
{
    internal interface IBaseValidator<T>
        where T : IEntity
    {
        /// <summary>
        /// Validates the data asynchronously and throws an exception if any validation errors occur.
        /// Returns a dictionary where the key is the error code, and the value is an array of error messages.
        /// </summary>
        /// <returns>
        /// A dictionary where:
        /// - The key is a string representing the error code.
        /// - The value is an array of strings, each representing a validation error message.
        /// </returns>
        Task<Dictionary<string, string[]>> ValidateAndThrowAsync(T entity);
    }
}
