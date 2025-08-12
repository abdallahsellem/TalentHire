using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace TalentHire.Shared.Kafka
{
    public interface IKafkaProducer
    {
        Task SendAsync<T>(string topic, string key, T message);
    }

    public class KafkaProducer : IKafkaProducer, IDisposable
    {
        private readonly IProducer<string, string> _producer;
        private readonly ILogger<KafkaProducer> _logger;

        public KafkaProducer(IConfiguration configuration, ILogger<KafkaProducer> logger)
        {
            _logger = logger;

            var config = new ProducerConfig
            {
                BootstrapServers = "kafka:9092"
            };

            _producer = new ProducerBuilder<string, string>(config).Build();
        }

        public async Task SendAsync<T>(string topic, string key, T message)
        {
            try
            {
                var json = JsonSerializer.Serialize(message);
                var kafkaMessage = new Message<string, string>
                {
                    Key = key,
                    Value = json,
                    Timestamp = Timestamp.Default
                };

                var result = await _producer.ProduceAsync(topic, kafkaMessage);
                _logger.LogInformation("Message sent to {Topic}:{Partition}:{Offset} with key {Key}",
                    topic, result.Partition, result.Offset, key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send message to {Topic} with key {Key}", topic, key);
                throw;
            }
        }

        public void Dispose()
        {
            _producer?.Flush(TimeSpan.FromSeconds(10));
            _producer?.Dispose();
        }
    }
}