using Domain.Entities;
using MediatR;
using Shared.Common;

namespace Application.MediatR.Queries.SeatConfigurationQueries
{
    public class SeatConfigurationGetPagedQuery : IRequest<PagedCollection<SeatConfiguration>>
    {
        public PageParameters PageParameters { get; set; }
    }
}
