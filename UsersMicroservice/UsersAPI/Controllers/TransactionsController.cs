using BusinessLogic.Services.Interfaces;
using DataAccess.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace UsersAPI.Controllers
{
    [ApiController]
    [Route("api/transactions")]
    public class TransactionsController : Controller
    {
        private readonly IUserTransactionService _userTransactionService;

        private const int PageSize = 10;

        public TransactionsController(IUserTransactionService userTransactionService)
        {
            _userTransactionService = userTransactionService;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserTransaction transaction, CancellationToken cancellationToken)
        {
            await _userTransactionService.CreateAsync(transaction, cancellationToken);

            return Ok();
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute]Guid id, [FromBody] UserTransaction transaction, CancellationToken cancellationToken)
        {
            if (id != transaction.Id)
                throw new ArgumentException("You are not allowed to modify this transaction.");

            await _userTransactionService.UpdateAsync(transaction, cancellationToken);

            return Ok();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            await _userTransactionService.DeleteAsync(id, cancellationToken);

            return Ok();
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var transaction = await _userTransactionService.GetByIdAsync(id, cancellationToken);
            if (transaction == null)
                return NotFound();

            return Ok(transaction);
        }

        [Authorize]
        [HttpGet("all")]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var transactions = await _userTransactionService.GetAllAsync(cancellationToken);

            return Ok(transactions);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetPaged([FromQuery] int pageNumber = 1, CancellationToken cancellationToken = default)
        {
            var transactions = await _userTransactionService.GetPagedAsync(pageNumber, PageSize, cancellationToken);

            return Ok(transactions);
        }
    }
}
