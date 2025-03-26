using Application.Repositories.Interfaces;

namespace Application.UnitOfWork.Interfaces
{
    public interface IUnitOfWork : IBaseUnitOfWork
    {
        IEventRepository EventRepository { get; }
        ISeatRepository SeatRepository { get; }
        IReviewRepository ReviewRepository { get; }
    }
}
