using Application.Specifications.Interfaces;
using System.Linq.Expressions;

namespace Application.Specifications.Common
{
    internal abstract class BaseSpecification<T> : ISpecification<T>
    {
        public abstract Expression<Func<T, bool>> ToExpression();

        public Func<T, bool> ToPredicate()
        {
            return ToExpression().Compile();
        }
    }
}
