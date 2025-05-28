using System.Linq.Expressions;

namespace Infrastructure.Specifications.Common
{
    internal abstract class BaseSpecification<T>
    {
        public abstract Expression<Func<T, bool>> ToExpression();

        public Func<T, bool> ToPredicate()
        {
            return ToExpression().Compile();
        }
    }
}
