using StackExchange.Redis;

namespace CacheRedis.CacheControl;
abstract class RedisInstanceExtend<T>(IConnectionMultiplexer redisConnection) : RedisBase<T>(redisConnection)
{
    private static Func<T, string> _getInstanceIdentity =
        inst => throw new InvalidOperationException($"Используй метод {nameof(SetInstanceIdentity)}");

    public static void SetInstanceIdentity(Func<T, string> func) => _getInstanceIdentity = func;

    public async Task SetInstance(T instance)
    {
        string instanceIdentity = _getInstanceIdentity(instance);
        await SetInstance(instance, instanceIdentity);
    }

    public async Task<T?> GetInstance<Tident>(Tident identity)
    {
        string? stringIdentity = identity?.ToString();
        if (stringIdentity is null)
        {
            return default;
        }

        return await GetInstance(stringIdentity);
    }

    public async Task RemoveInstance<Tident>(Tident identity)
    {
        string? stringIdentity = identity?.ToString();
        if (stringIdentity is null)
        {
            return;
        }

        await RemoveInstance(stringIdentity);
    }
}
