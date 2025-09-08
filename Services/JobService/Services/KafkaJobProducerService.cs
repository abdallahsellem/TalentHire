using TalentHire.Shared.Kafka;
using TalentHire.Shared.Kafka.Models;

namespace TalentHire.Services.JobService.Services;

public class KafkaJobProducerService
{
    private readonly IKafkaProducer _kafkaProducer;
    private readonly ILogger<KafkaJobProducerService> _logger;

    public KafkaJobProducerService(IKafkaProducer kafkaProducer, ILogger<KafkaJobProducerService> logger)
    {
        _kafkaProducer = kafkaProducer;
        _logger = logger;
    }

    public async Task CreateJobAsync(int userId, int jobId)
    {

        var jobEvent = new JobCreatedEvent
        {
            JobId = jobId,
            EmployerId = userId,
            Status = JobStatus.Open,
        };

        await _kafkaProducer.SendAsync("jobs-topic", jobId.ToString(), jobEvent);
        _logger.LogInformation("Job created and event published: {JobId}", jobId);
    }
}