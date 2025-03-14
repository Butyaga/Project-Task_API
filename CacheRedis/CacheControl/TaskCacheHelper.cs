using StackExchange.Redis;
using API_Abstract.POCO;
using CacheRedis.Models.Converters;
using System.Text.Json;
using CacheRedis.Models;

namespace CacheRedis.CacheControl;
public class TaskCacheHelper : RedisInstanceExtend<ITask>
{
    public TaskCacheHelper(IConnectionMultiplexer redisConnection) : base(redisConnection)
    {
        SetInstanceIdentity(task => task.Id.ToString());
        JsonSerializerOptions options = new()
        {
            Converters = {
            new InterfaceConverterFactory(typeof(ProjectDTO), typeof(IProject)),
            new InterfaceConverterFactory(typeof(ProjectTaskDTO), typeof(ITask)),
            new EnumerableConverterFactory(typeof(IProject)),
            new EnumerableConverterFactory(typeof(ITask))
            }
        };
        AddJsonDeserializationOptions(options);
    }

    const string _allTasksIdentity = "AllTasks";

    public async Task SetCollection(IEnumerable<ITask> collection, bool? isCompleted, int? projectId)
    {
        string collectionIdentity = GetCollectionIdentity(isCompleted, projectId);
        await SetCollection(collection, collectionIdentity);
    }

    public async Task<IEnumerable<ITask>?> GetCollection(bool? isCompleted, int? projectId)
    {
        string collectionIdentity = GetCollectionIdentity(isCompleted, projectId);
        return await GetCollection(collectionIdentity);
    }

    public async Task RemoveCollection(bool? isCompleted, int? projectId)
    {
        string collectionIdentity = GetCollectionIdentity(isCompleted, projectId);
        await RemoveCollection(collectionIdentity);
    }

    private static string GetCollectionIdentity(bool? isCompleted, int? projectId)
    {
        string? result = AddValueToString(projectId);
        result = AddValueToString(isCompleted, result);

        if (string.IsNullOrEmpty(result))
        {
            return _allTasksIdentity;
        }
        return result;
    }

    private static string? AddValueToString(object? nullableValue, string? str = null)
    {
        if (nullableValue is null)
        {
            return str;
        }

        if (string.IsNullOrEmpty(str))
        {
            return nullableValue.ToString();
        }

        return str + "_" + nullableValue.ToString();
    }
}
