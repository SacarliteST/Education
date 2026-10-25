using Education.Kafka.Producing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Education.Kafka;

/// <summary>Регистрация Kafka-инфраструктуры в контейнере зависимостей.</summary>
public static class KafkaServiceCollectionExtensions
{
    /// <summary>Регистрирует <see cref="KafkaOptions"/> и открытый generic-продюсер. Консьюмеров здесь нет — их регистрирует вызывающий проект.</summary>
    /// <param name="services">Коллекция сервисов.</param>
    /// <param name="configuration">Конфигурация приложения.</param>
    /// <returns>Та же коллекция сервисов.</returns>
    public static IServiceCollection AddKafkaMessaging(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<KafkaOptions>().BindConfiguration(KafkaOptions.SectionKey);
        services.AddSingleton(typeof(IKafkaProducer<>), typeof(KafkaProducer<>));
        return services;
    }
}
