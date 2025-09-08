using TalentHire.Shared.Kafka;
using TalentHire.Shared.Kafka.Models;

namespace TalentHire.Services.JobService.Services;

public class KafkaApplicationConsumerService : KafkaConsumer<ApplicationCreatedEvent>
{
    private readonly ILogger<KafkaApplicationConsumerService> _logger;

    public KafkaApplicationConsumerService(ILogger<KafkaApplicationConsumerService> logger):base(
        logger,
        "applications-topic",
        "job-service-group")    
    {

        _logger = logger;
    }
    protected override async Task HandleMessage(string key, ApplicationCreatedEvent eventData)
    {
        await ConsumeJobAsync(eventData.ApplicationId, eventData.UserId,eventData.Status);
    }

    public async Task ConsumeJobAsync(int applicationId, int userId,string status)
    {

        var jobEvent = new ApplicationCreatedEvent
        {
            ApplicationId = applicationId,
            UserId = userId,
            Status = status,
        };

        _logger.LogInformation("Application Consumed : {ApplicationId}", applicationId);
    }
}