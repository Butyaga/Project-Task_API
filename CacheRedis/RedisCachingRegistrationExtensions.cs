using CacheRedis.AbstractRealisation;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace CacheRedis;
public static class RedisCachingRegistrationExtensions
{
    public static IServiceCollection AddRedisCaching(this IServiceCollection services, string? configuration)
    {
        if (string.IsNullOrEmpty(configuration))
        {
            throw new ArgumentException("Не задана конфигурация подключения сервера Redis");
        }
        services.AddSingleton<IConnectionMultiplexer>(sp => ConnectionMultiplexer.Connect(configuration));
        services.AddScoped<IProjectManagerProxy, ProjectManagerProxy>();
        return services;
    }
}
