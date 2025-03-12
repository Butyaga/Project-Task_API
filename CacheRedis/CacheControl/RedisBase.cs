using StackExchange.Redis;
using System.Text.Json;

namespace CacheRedis.CacheControl;
abstract class RedisBase<T>
{
    #region Template
    const string _className = "{className}";
    const string _identity = "{Identity}";

    private readonly string _collectionSearchPattern = "Coll_" + _className + "_*";
    private readonly string _templateInstance = _className + "_" + _identity;
    private readonly string _templateCollection = "Coll_" + _className + "_" + _identity;
    #endregion

    protected RedisBase(IConnectionMultiplexer redisConnection)
    {
        string className = typeof(T).Name;
        _collectionSearchPattern = _collectionSearchPattern.Replace(_className, className);
        _templateInstance = _templateInstance.Replace(_className, className);
        _templateCollection = _templateCollection.Replace(_identity, className);
        _redisConnection = redisConnection;
        _redisDB = redisConnection.GetDatabase();
    }

    private readonly IConnectionMultiplexer _redisConnection;
    private readonly IDatabase _redisDB;
    private readonly TimeSpan _expiry = TimeSpan.FromMinutes(5);

    #region Public interface
    public async Task SetInstance(T instance, string identity)
    {
        string fullIdentity = GetFullInstanceIdentity(identity);
        string stringData = JsonSerializer.Serialize(instance);

        await Task.WhenAll([
            _redisDB.StringSetAsync(fullIdentity, stringData, _expiry),
            DeleteAllCollectionEntry()
            ]);
    }

    public async Task<T?> GetInstance(string identity)
    {
        string fullIdentity = GetFullInstanceIdentity(identity);

        string? data = await _redisDB.StringGetAsync(fullIdentity);
        if (data is null)
        {
            return default;
        }

        return JsonSerializer.Deserialize<T>(data);
    }

    public async Task RemoveInstance(string identity)
    {
        string fullIdentity = GetFullInstanceIdentity(identity);

        await Task.WhenAll([
            _redisDB.KeyDeleteAsync(fullIdentity),
            DeleteAllCollectionEntry()
            ]);
    }

    public async Task SetCollection(IEnumerable<T> collection, string identity)
    {
        string fullIdentity = GetFullCollectionIdentity(identity);
        string stringData = JsonSerializer.Serialize(collection);

        await _redisDB.StringSetAsync(fullIdentity, stringData, _expiry);
    }

    public async Task<IEnumerable<T>?> GetCollection(string identity)
    {
        string fullIdentity = GetFullCollectionIdentity(identity);

        string? data = await _redisDB.StringGetAsync(fullIdentity);
        if (data is null)
        {
            return null;
        }

        return JsonSerializer.Deserialize<IEnumerable<T>>(data);
    }

    public async Task RemoveCollection(string identity)
    {
        string fullIdentity = GetFullCollectionIdentity(identity);
        await _redisDB.KeyDeleteAsync(fullIdentity);
    }
    #endregion

    #region Private methods
    private string GetFullInstanceIdentity(string identity)
    {
        //string instanceIdentity = _getInstanceIdentity(instance);
        return _templateInstance.Replace(_identity, identity);
    }

    private string GetFullCollectionIdentity(string identity)
    {
        return _templateCollection.Replace(_identity, identity);
    }

    private async Task DeleteAllCollectionEntry()
    {
        IServer server = _redisConnection.GetServer(_redisConnection.GetEndPoints().First());
        IAsyncEnumerable<RedisKey> keys = server.KeysAsync(database: _redisDB.Database, pattern: _collectionSearchPattern);

        await foreach (RedisKey key in keys)
        {
            await _redisDB.KeyDeleteAsync(key);
        }
    }
    #endregion
}
