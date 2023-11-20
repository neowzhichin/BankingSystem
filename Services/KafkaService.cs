using Confluent.Kafka;
using Newtonsoft.Json;
public class KafkaConsumerService : IKafkaConsumerService
{
    private readonly IConsumer<Ignore, string> _consumer;
    private readonly IProducer<Null, string> _producer;
    public KafkaConsumerService(IConfiguration configuration)
    {
        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = configuration["Kafka:BootstrapServers"],
            GroupId = configuration["Kafka:GroupId"],
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false
        };

        var producerConfig = new ProducerConfig
        {
            BootstrapServers = "localhost:9092",
        };

        _producer = new ProducerBuilder<Null, string>(producerConfig).Build();
        _consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();
    }

    public async Task<IEnumerable<Transaction>> ConsumeTransactionsAsync(string topic, int limit, int page)
    {   
        var transactions = new List<Transaction>();
        var watermarkOffsets = _consumer.QueryWatermarkOffsets(new TopicPartition(topic, new Partition(0)), new TimeSpan(0,0,10));
        long maxOffset = watermarkOffsets.High;
        
        int curOffset=(page-1)*limit;
        if(curOffset>=maxOffset){
            return new List<Transaction>();
        }
        
        _consumer.Assign(new TopicPartitionOffset(topic, 0, new Offset(curOffset)));

        try
        {
            while (true)
            {
                var consumeResult = _consumer.Consume();
                transactions.Add(JsonConvert.DeserializeObject<Transaction>(consumeResult.Message.Value));
                curOffset++;

                if (consumeResult.IsPartitionEOF||curOffset==maxOffset||curOffset==page*limit)
                    break;   
            }
        }
        catch (OperationCanceledException)
        {
            // The consumer was closed, or the cancellation token was canceled
        }
        finally
        {
            _consumer.Close();
        }

        return transactions;
    }

    public void ProduceTransaction(string topic, string transaction)
    {
        _producer.Produce(topic, new Message<Null, string> { Value = transaction });
        _producer.Flush(TimeSpan.FromSeconds(10));
    }
    
}