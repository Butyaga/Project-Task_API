using StackExchange.Redis;
using API_Abstract.POCO;
using CacheRedis.Models.Converters;
using System.Text.Json;
using CacheRedis.Models;

namespace CacheRedis.CacheControl;
public class ProjectCacheHelper : RedisInstanceExtend<IProject>
{
    public ProjectCacheHelper(IConnectionMultiplexer redisConnection) : base(redisConnection)
    {
        SetInstanceIdentity(project => project.Id.ToString());
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

    const string _allProjectsIdentity = "AllProjects";

    public async Task SetCollection(IEnumerable<IProject> collection, int pageIndex, int pageSize)
    {
        string collectionIdentity = GetCollectionIdentity(pageIndex, pageSize);
        await SetCollection(collection, collectionIdentity);
    }

    public async Task SetCollection(IEnumerable<IProject> collection)
    {
        await SetCollection(collection, _allProjectsIdentity);
    }

    public async Task<IEnumerable<IProject>?> GetCollection(int pageIndex, int pageSize)
    {
        string collectionIdentity = GetCollectionIdentity(pageIndex, pageSize);
        return await GetCollection(collectionIdentity);
    }

    public async Task<IEnumerable<IProject>?> GetCollectionAllProjects()
    {
        return await GetCollection(_allProjectsIdentity);
    }

    public async Task RemoveCollection(int pageIndex, int pageSize)
    {
        string collectionIdentity = GetCollectionIdentity(pageIndex, pageSize);
        await RemoveCollection(collectionIdentity);
    }

    public async Task RemoveCollectionAllProjects()
    {
        await RemoveCollection(_allProjectsIdentity);
    }

    private static string GetCollectionIdentity(int pageIndex, int pageSize) { return $"Idx{pageIndex}_Sz{pageSize}"; }
}
