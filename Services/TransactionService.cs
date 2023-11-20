using Newtonsoft.Json;

public class TransactionService : ITransactionService
{
    private readonly IKafkaConsumerService _kafkaConsumerService;

    public TransactionService(IKafkaConsumerService kafkaConsumerService)
    {
        _kafkaConsumerService = kafkaConsumerService;
    }

    public async Task<IEnumerable<Transaction>> GetTransactions(string topic, int limit, int page)
    {
        if(page<1){
            page=1;
        }

        if(limit<1){
            limit=5;
        }
        
        // Consume transactions from Kafka (this is a simplified example)
        var kafkaTransactions = await _kafkaConsumerService.ConsumeTransactionsAsync(topic, limit, page);

        // Combine and return transactions
        return kafkaTransactions;
    }

    public void NewTransactions(string topic, Transaction request)
    {
        _kafkaConsumerService.ProduceTransaction(topic, JsonConvert.SerializeObject(request));
    }
}