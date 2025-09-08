using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace TalentHire.Shared.Kafka
{

public abstract class KafkaConsumer<T> : BackgroundService
{
    private readonly IConsumer<string, string> _consumer;
    private readonly ILogger _logger;
    private readonly string _topic;
    private readonly string _groupId;

    protected KafkaConsumer(ILogger logger, string topic, string groupId)
    {
        _logger = logger;
        _topic = topic;
        _groupId = groupId;

        var config = new ConsumerConfig
        {
            BootstrapServers = "kafka:9092",
            GroupId = groupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = true
        };

        _consumer = new ConsumerBuilder<string, string>(config).Build();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _consumer.Subscribe(_topic);
        _logger.LogInformation("Kafka consumer started for topic: {Topic}, GroupId: {GroupId}", _topic, _groupId);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var consumeResult = _consumer.Consume(stoppingToken);

                if (consumeResult?.Message != null)
                {
                    await ProcessMessage(consumeResult.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message from topic {Topic}", _topic);
                await Task.Delay(1000, stoppingToken); // Brief delay on error
            }
        }
    }

    private async Task ProcessMessage(Message<string, string> message)
    {
         _logger.LogInformation("Test ****************************************************************");
         _logger.LogInformation("Message Consumed: {Message}", message.Value);
        try
            {

                var eventData = JsonSerializer.Deserialize<T>(message.Value);
                if (eventData != null)
                {
                    await HandleMessage(message.Key, eventData);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process message: {Message}", message.Value);
            }
    }

    protected abstract Task HandleMessage(string key, T eventData);

    public override void Dispose()
    {
        _consumer?.Close();
        _consumer?.Dispose();
        base.Dispose();
    }
}
}