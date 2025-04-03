using Application.Repositories.Interfaces;

namespace Application.UnitOfWork.Interfaces
{
    public interface IUnitOfWork : IBaseUnitOfWork
    {
        IEventRepository EventRepository { get; }
        IEventCommentRepository EventCommentRepository { get; }
        ISeatRepository SeatRepository { get; }
        ISeatConfigurationRepository SeatConfigurationRepository { get; }
    }
}
