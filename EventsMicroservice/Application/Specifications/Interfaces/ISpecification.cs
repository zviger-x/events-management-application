using System.Linq.Expressions;

namespace Application.Specifications.Interfaces
{
    public interface ISpecification<T>
    {
        /// <summary>
        /// Converts the specification to an expression that can be used for querying.
        /// </summary>
        /// <returns>An expression representing the criteria of the specification.</returns>
        Expression<Func<T, bool>> ToExpression();

        /// <summary>
        /// Converts the specification to a compiled predicate function.
        /// </summary>
        /// <returns>A function that evaluates whether an entity satisfies the specification.</returns>
        Func<T, bool> ToPredicate();
    }
}
