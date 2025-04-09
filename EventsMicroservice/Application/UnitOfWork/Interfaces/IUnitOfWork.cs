using Application.Repositories.Interfaces;
using Shared.UnitOfWork;

namespace Application.UnitOfWork.Interfaces
{
    public interface IUnitOfWork : IBaseUnitOfWork
    {
        IEventRepository EventRepository { get; }
        IEventUserRepository EventUserRepository { get; }
        IEventCommentRepository EventCommentRepository { get; }
        ISeatRepository SeatRepository { get; }
        ISeatConfigurationRepository SeatConfigurationRepository { get; }
    }
}
