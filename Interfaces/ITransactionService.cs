public interface ITransactionService
{
    Task<IEnumerable<Transaction>> GetTransactions(string topic, int limit, int page);
    void NewTransactions(string Topic, Transaction request);
}