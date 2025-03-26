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
        private Lazy<IReviewRepository> _reviewRepository;

        public UnitOfWork(EventDbContext context, IServiceProvider serviceProvider)
            : base(context, serviceProvider)
        {
            _eventRepository = new Lazy<IEventRepository>(_serviceProvider.GetRequiredService<IEventRepository>);
            _seatRepository = new Lazy<ISeatRepository>(_serviceProvider.GetRequiredService<ISeatRepository>);
            _reviewRepository = new Lazy<IReviewRepository>(_serviceProvider.GetRequiredService<IReviewRepository>);
        }

        public IEventRepository EventRepository => _eventRepository.Value;

        public ISeatRepository SeatRepository => _seatRepository.Value;

        public IReviewRepository ReviewRepository => _reviewRepository.Value;

        public override void Dispose()
        {
            base.Dispose();

            if (_eventRepository.IsValueCreated)
                _eventRepository.Value.Dispose();

            if (_seatRepository.IsValueCreated)
                _seatRepository.Value.Dispose();

            if (_reviewRepository.IsValueCreated)
                _reviewRepository.Value.Dispose();
        }
    }
}
