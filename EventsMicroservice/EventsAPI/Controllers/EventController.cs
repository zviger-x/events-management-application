using Application.Contracts;
using Application.MediatR.Commands.EventCommands;
using Application.MediatR.Commands.ReviewCommands;
using Application.MediatR.Queries.EventQueries;
using Application.MediatR.Queries.ReviewQueries;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EventsAPI.Controllers
{
    [ApiController]
    [Route("api/events")]
    public class EventController : Controller
    {
        private readonly IMediator _mediator;

        private const int EventsPageSize = 10;
        private const int ReviewsPageSize = 10;

        public EventController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// ---- Events ----

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateEventDTO eventToCreate, CancellationToken cancellationToken)
        {
            var command = new EventCreateCommand { Event = eventToCreate };
            var createdEventId = await _mediator.Send(command);

            return Ok(createdEventId);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync([FromRoute] Guid id, [FromBody] UpdateEventDTO eventToUpdate, CancellationToken cancellationToken)
        {
            if (id != eventToUpdate.Id)
                throw new ArgumentException("You are not allowed to modify this event.");

            var command = new EventUpdateCommand { Event = eventToUpdate };
            await _mediator.Send(command);

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            var command = new EventDeleteCommand { Id = id };
            await _mediator.Send(command);

            return Ok();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var query = new EventGetByIdQuery { Id = id };
            var @event = await _mediator.Send(query);

            if (@event == null)
                return NotFound();

            return Ok(@event);
        }

        [HttpGet]
        public async Task<IActionResult> GetPagedAsync([FromQuery] int pageNumber = 1, CancellationToken cancellationToken = default)
        {
            var query = new EventGetPagedQuery { PageNumber = pageNumber, PageSize = EventsPageSize };
            var page = await _mediator.Send(query);

            return Ok(page);
        }

        /// ---- Event reviews ----

        [HttpPost("{eventId}/reviews")]
        public async Task<IActionResult> CreateReviewAsync([FromRoute] Guid eventId, [FromBody] Review reviewToCreate, CancellationToken token)
        {
            // TODO: Добавить проверку, что пользователь является участником ивента.

            if (eventId != reviewToCreate.EventId)
                throw new ArgumentException("You are not allowed to create a review for this event.");

            var command = new ReviewCreateCommand { Review = reviewToCreate };
            var createdReviewId = await _mediator.Send(command);

            return Ok(createdReviewId);
        }

        [HttpPut("{eventId}/reviews/{reviewId}")]
        public async Task<IActionResult> UpdateReviewAsync([FromRoute] Guid eventId, [FromRoute] Guid reviewId, [FromBody] Review reviewToUpdate, CancellationToken token)
        {
            // TODO: Добавить проверку, что пользователь является участником ивента.
            // TODO: Добавить проверку, что пользователь является создателем отзыва.

            if (eventId != reviewToUpdate.EventId)
                throw new ArgumentException("You are not allowed to edit the review for this event.");

            if (reviewId != reviewToUpdate.Id)
                throw new ArgumentException("You are not allowed to modify this review.");

            var command = new ReviewUpdateCommand { Review = reviewToUpdate };
            await _mediator.Send(command);

            return Ok();
        }


        [HttpDelete("{eventId}/reviews/{reviewId}")]
        public async Task<IActionResult> DeleteReviewAsync([FromRoute] Guid eventId, [FromRoute] Guid reviewId, CancellationToken token)
        {
            // TODO: Добавить проверку, что пользователь является участником ивента.
            // TODO: Добавить проверку, что пользователь является создателем отзыва.

            // if (id != reviewToUpdate.EventId)
            //     throw new ArgumentException("You are not allowed to edit the review for this event.");

            var command = new ReviewDeleteCommand { Id = reviewId };
            await _mediator.Send(command);

            return Ok();
        }

        [HttpGet("{eventId}/reviews")]
        public async Task<IActionResult> GetPagedReviewsAsync([FromRoute] Guid eventId, [FromQuery] int pageNumber = 1, CancellationToken cancellationToken = default)
        {
            var command = new ReviewGetPagedByEventQuery { EventId = eventId, PageNumber = pageNumber, PageSize = ReviewsPageSize };
            var reviewsPage = await _mediator.Send(command);

            return Ok(reviewsPage);
        }
    }
}
