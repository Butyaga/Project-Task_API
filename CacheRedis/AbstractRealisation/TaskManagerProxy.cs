using API_Abstract.DTO;
using API_Abstract.Managers;
using API_Abstract.POCO;
using CacheRedis.CacheControl;
using StackExchange.Redis;

namespace CacheRedis.AbstractRealisation;
public class TaskManagerProxy(ITaskManager _taskManager, IConnectionMultiplexer redisConnection) : ITaskManagerProxy
{
    private readonly TaskCacheHelper _taskCacheHelper = new(redisConnection);

    public async Task<ITask> CreateTaskAsync(ITaskDTO taskDTO)
    {
        ITask task = await _taskManager.CreateTaskAsync(taskDTO);
        await _taskCacheHelper.SetInstance(task);
        return task;
    }

    public async Task<bool> DeleteTaskAsync(int id)
    {
        Task<bool> task = _taskManager.DeleteTaskAsync(id);
        await _taskCacheHelper.RemoveInstance(id);
        return await task;
    }

    public async Task<ITask?> GetTaskAsync(int id)
    {
        ITask? cachedResult = await _taskCacheHelper.GetInstance(id);
        if (cachedResult is not null)
        {
            return cachedResult;
        }

        ITask? task = await _taskManager.GetTaskAsync(id);
        if (task is not null)
        {
            await _taskCacheHelper.SetInstance(task);
        }

        return task;
    }

    public async Task<IEnumerable<ITask>> GetTasksAsync(bool? isCompleted, int? projectId)
    {
        IEnumerable<ITask>? cachedResult = await _taskCacheHelper.GetCollection(isCompleted, projectId);
        if (cachedResult is not null)
        {
            return cachedResult;
        }

        IEnumerable<ITask> result = await _taskManager.GetTasksAsync(isCompleted, projectId);
        await _taskCacheHelper.SetCollection(result, isCompleted, projectId);
        return result;
    }

    public async Task<bool> UpdateTaskAsync(int id, ITaskDTO taskDTO)
    {
        Task<bool> task = _taskManager.UpdateTaskAsync(id, taskDTO);
        await _taskCacheHelper.RemoveInstance(id);
        return await task;
    }
}
