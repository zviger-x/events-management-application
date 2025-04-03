using Application.Repositories.Interfaces;
using Application.UnitOfWork.Interfaces;
using Infrastructure.Contexts;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.UnitOfWork
{
    public class UnitOfWork : BaseUnitOfWork, IUnitOfWork
    {
        private Lazy<IEventRepository> _eventRepository;
        private Lazy<IEventCommentRepository> _eventCommentRepository;
        private Lazy<ISeatRepository> _seatRepository;
        private Lazy<ISeatConfigurationRepository> _seatConfigurationRepository;

        public UnitOfWork(EventDbContext context, TransactionContext transactionContext, IServiceProvider serviceProvider)
            : base(context, transactionContext, serviceProvider)
        {
            _eventRepository = new Lazy<IEventRepository>(_serviceProvider.GetRequiredService<IEventRepository>);
            _eventCommentRepository = new Lazy<IEventCommentRepository>(_serviceProvider.GetRequiredService<IEventCommentRepository>);
            _seatRepository = new Lazy<ISeatRepository>(_serviceProvider.GetRequiredService<ISeatRepository>);
            _seatConfigurationRepository = new Lazy<ISeatConfigurationRepository>(_serviceProvider.GetRequiredService<ISeatConfigurationRepository>);
        }

        public IEventRepository EventRepository => _eventRepository.Value;

        public IEventCommentRepository EventCommentRepository => _eventCommentRepository.Value;

        public ISeatRepository SeatRepository => _seatRepository.Value;

        public ISeatConfigurationRepository SeatConfigurationRepository => _seatConfigurationRepository.Value;

        public override void Dispose()
        {
            base.Dispose();

            if (_eventRepository.IsValueCreated)
                _eventRepository.Value.Dispose();

            if (_eventCommentRepository.IsValueCreated)
                _eventCommentRepository.Value.Dispose();

            if (_seatRepository.IsValueCreated)
                _seatRepository.Value.Dispose();

            if (_seatConfigurationRepository.IsValueCreated)
                _seatConfigurationRepository.Value.Dispose();
        }
    }
}
