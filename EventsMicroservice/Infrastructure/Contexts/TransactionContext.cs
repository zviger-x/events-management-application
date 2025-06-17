using MongoDB.Driver;

namespace Infrastructure.Contexts
{
    public class TransactionContext
    {
        public IClientSessionHandle Session { get; set; }
    }
}
