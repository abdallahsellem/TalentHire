using Microsoft.Extensions.DependencyInjection;

namespace TalentHire.Shared.Kafka
{



    public static class ServicesCollections
    {
        public static IServiceCollection AddKafka(this IServiceCollection services)
        {
            services.AddSingleton<IKafkaProducer, KafkaProducer>();
            return services;
        }
    }
}