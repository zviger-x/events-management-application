using BusinessLogic.Services.Interfaces;
using DataAccess.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace UsersAPI.Controllers
{
    // TODO: Создать gRPC сервис для работы с транзакциями
    // Этот класс для демонстрации и проверки корректности работы CRUD операций с транзакциями.
    // Он будет перенесён в gRPC сервис, потому что пользователю нет смысла взаимодействовать с этими методами.
    [ApiController]
    [Route("api/transactions")]
    public class TransactionsController : Controller
    {
        private const int PageSize = 10;

        private readonly IUserTransactionService _userTransactionService;

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
        [HttpPut("{transactionId}")]
        public async Task<IActionResult> Update([FromRoute] Guid transactionId, [FromBody] UserTransaction transaction, CancellationToken cancellationToken)
        {
            await _userTransactionService.UpdateAsync(transactionId, transaction, cancellationToken);

            return Ok();
        }

        [Authorize]
        [HttpDelete("{transactionId}")]
        public async Task<IActionResult> Delete([FromRoute] Guid transactionId, CancellationToken cancellationToken)
        {
            await _userTransactionService.DeleteAsync(transactionId, cancellationToken);

            return NoContent();
        }

        [Authorize]
        [HttpGet("{transactionId}")]
        public async Task<IActionResult> GetById([FromRoute] Guid transactionId, CancellationToken cancellationToken)
        {
            var transaction = await _userTransactionService.GetByIdAsync(transactionId, cancellationToken);

            return Ok(transaction);
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
