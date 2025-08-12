using TalentHire.Shared.Kafka;
using TalentHire.Shared.Kafka.Models;

namespace TalentHire.Services.ApplicationsService.Services;

public class KafkaApplicationService
{
    private readonly IKafkaProducer _kafkaProducer;
    private readonly ILogger<KafkaApplicationService> _logger;

    public KafkaApplicationService(IKafkaProducer kafkaProducer, ILogger<KafkaApplicationService> logger)
    {
        _kafkaProducer = kafkaProducer;
        _logger = logger;
    }

    public async Task CreateApplicationAsync(int userId, int jobId)
    {
        var applicationId = Guid.NewGuid().ToString();

        var applicationEvent = new ApplicationCreatedEvent
        {
            ApplicationId = applicationId,
            UserId = userId,
            JobId = jobId,
            Status = "Created",
            EventType = "ApplicationCreated"
        };

        await _kafkaProducer.SendAsync("applications-topic", applicationId, applicationEvent);
        _logger.LogInformation("Application created and event published: {ApplicationId}", applicationId);
    }
}