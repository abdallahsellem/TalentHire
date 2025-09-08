using TalentHire.Shared.Kafka;
using TalentHire.Shared.Kafka.Models;

namespace TalentHire.Services.ApplicationsService.Services;

public class KafkaApplicationProducerService
{
    private readonly IKafkaProducer _kafkaProducer;
    private readonly ILogger<KafkaApplicationProducerService> _logger;

    public KafkaApplicationProducerService(IKafkaProducer kafkaProducer, ILogger<KafkaApplicationProducerService> logger)
    {
        _kafkaProducer = kafkaProducer;
        _logger = logger;
    }

    public async Task CreateApplicationAsync(int applicationId, int userId, int jobId)
    {

        var applicationEvent = new ApplicationCreatedEvent
        {
            ApplicationId = applicationId,
            UserId = userId,
            JobId = jobId,
            Status = "Created",
            EventType = "ApplicationCreated"
        };

        await _kafkaProducer.SendAsync("applications-topic", applicationId.ToString(), applicationEvent);
        _logger.LogInformation("Application created and event published: {ApplicationId}", applicationId);
    }
}