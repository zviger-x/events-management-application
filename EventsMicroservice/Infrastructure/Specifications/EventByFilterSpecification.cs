using Domain.Entities;
using Infrastructure.Specifications.Common;
using System.Linq.Expressions;

namespace Infrastructure.Specifications
{
    internal class EventByFilterSpecification : BaseSpecification<Event>
    {
        private readonly string _name;
        private readonly string _description;
        private readonly string _location;
        private readonly DateTimeOffset? _fromDate;
        private readonly DateTimeOffset? _toDate;

        public EventByFilterSpecification(
            string name,
            string description,
            string location,
            DateTimeOffset? fromDate,
            DateTimeOffset? toDate)
        {
            _name = name;
            _description = description;
            _location = location;
            _fromDate = fromDate;
            _toDate = toDate;
        }

        public override Expression<Func<Event, bool>> ToExpression()
        {
            return e =>
                (string.IsNullOrEmpty(_name) || e.Name.Contains(_name)) &&
                (string.IsNullOrEmpty(_description) || e.Description.Contains(_description)) &&
                (string.IsNullOrEmpty(_location) || e.Location.Contains(_location)) &&
                (!_fromDate.HasValue || e.StartDate >= _fromDate.Value) &&
                (!_toDate.HasValue || e.EndDate <= _toDate.Value);
        }
    }
}
