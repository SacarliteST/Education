using Education.Kafka.Producing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Education.Kafka;

public static class KafkaServiceCollectionExtensions
{
    /// <summary>Регистрирует <see cref="KafkaOptions"/> и открытый generic-продюсер. Консьюмеров здесь нет — их регистрирует вызывающий проект.</summary>
    public static IServiceCollection AddKafkaMessaging(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<KafkaOptions>().BindConfiguration(KafkaOptions.SectionKey);
        services.AddSingleton(typeof(IKafkaProducer<>), typeof(KafkaProducer<>));
        return services;
    }
}
