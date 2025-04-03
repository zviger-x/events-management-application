using Application.Repositories.Interfaces;
using Application.UnitOfWork.Interfaces;
using Infrastructure.Contexts;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.UnitOfWork
{
    public class UnitOfWork : BaseUnitOfWork, IUnitOfWork
    {
        private Lazy<IEventRepository> _eventRepository;
        private Lazy<ISeatRepository> _seatRepository;
        private Lazy<IEventCommentRepository> _eventCommentRepository;

        public UnitOfWork(EventDbContext context, IServiceProvider serviceProvider)
            : base(context, serviceProvider)
        {
            _eventRepository = new Lazy<IEventRepository>(_serviceProvider.GetRequiredService<IEventRepository>);
            _seatRepository = new Lazy<ISeatRepository>(_serviceProvider.GetRequiredService<ISeatRepository>);
            _eventCommentRepository = new Lazy<IEventCommentRepository>(_serviceProvider.GetRequiredService<IEventCommentRepository>);
        }

        public IEventRepository EventRepository => _eventRepository.Value;

        public ISeatRepository SeatRepository => _seatRepository.Value;

        public IEventCommentRepository EventCommentRepository => _eventCommentRepository.Value;

        public override void Dispose()
        {
            base.Dispose();

            if (_eventRepository.IsValueCreated)
                _eventRepository.Value.Dispose();

            if (_seatRepository.IsValueCreated)
                _seatRepository.Value.Dispose();

            if (_eventCommentRepository.IsValueCreated)
                _eventCommentRepository.Value.Dispose();
        }
    }
}
