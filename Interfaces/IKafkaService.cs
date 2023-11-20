public interface IKafkaConsumerService
{
    Task<IEnumerable<Transaction>> ConsumeTransactionsAsync(string topic, int limit, int page);
    void ProduceTransaction(string topic, string transaction);
}