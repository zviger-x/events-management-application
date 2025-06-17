using BusinessLogic.Contracts;
using BusinessLogic.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Common;
using Shared.Extensions;

namespace UsersAPI.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/transactions")]
    public class TransactionsController : ControllerBase
    {
        private const int PageSize = 10;

        private readonly IUserTransactionService _userTransactionService;

        public TransactionsController(IUserTransactionService userTransactionService)
        {
            _userTransactionService = userTransactionService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserTransactionDto transaction, CancellationToken cancellationToken)
        {
            var createdId = await _userTransactionService.CreateAsync(transaction, cancellationToken);

            return StatusCode(StatusCodes.Status201Created, new { Id = createdId });
        }

        [HttpPut("{transactionId}")]
        public async Task<IActionResult> Update([FromRoute] Guid transactionId, [FromBody] UpdateUserTransactionDto transaction, CancellationToken cancellationToken)
        {
            await _userTransactionService.UpdateAsync(transactionId, transaction, cancellationToken);

            return Ok();
        }

        [HttpDelete("{transactionId}")]
        public async Task<IActionResult> Delete([FromRoute] Guid transactionId, CancellationToken cancellationToken)
        {
            await _userTransactionService.DeleteAsync(transactionId, cancellationToken);

            return NoContent();
        }

        [HttpGet("{transactionId}")]
        public async Task<IActionResult> GetById([FromRoute] Guid transactionId, CancellationToken cancellationToken)
        {
            var transaction = await _userTransactionService.GetByIdAsync(transactionId, cancellationToken);

            return Ok(transaction);
        }

        [HttpGet]
        public async Task<IActionResult> GetPaged([FromQuery] int pageNumber = 1, CancellationToken cancellationToken = default)
        {
            var pageParameters = new PageParameters { PageNumber = pageNumber, PageSize = PageSize };

            var transactions = await _userTransactionService.GetPagedAsync(pageParameters, cancellationToken);

            return Ok(transactions);
        }

        [HttpGet("/api/users/{userId}/transactions")]
        public async Task<IActionResult> GetUserTransactions([FromRoute] Guid userId, CancellationToken token)
        {
            var currentUserId = User.GetUserIdOrThrow();
            var isAdmin = User.IsAdminOrThrow();

            var transactions = await _userTransactionService.GetByUserIdAsync(userId, currentUserId, isAdmin, token);

            return Ok(transactions);
        }

    }
}
