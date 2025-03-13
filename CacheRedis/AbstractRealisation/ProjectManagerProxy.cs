using API_Abstract.DTO;
using API_Abstract.Managers;
using API_Abstract.POCO;
using CacheRedis.CacheControl;
using StackExchange.Redis;

namespace CacheRedis.AbstractRealisation;
public class ProjectManagerProxy(IProjectManager _projectManager, IConnectionMultiplexer redisConnection) : IProjectManagerProxy
{
    private readonly ProjectCacheHelper _projectCacheHelper = new(redisConnection);

    public async Task<IProject> CreateProjectAsync(IProjectDTO projectDTO)
    {
        IProject project = await _projectManager.CreateProjectAsync(projectDTO);
        await _projectCacheHelper.SetInstance(project);
        return project;
    }

    public async Task<bool> DeleteProjectAsync(int Id)
    {
        Task<bool> task = _projectManager.DeleteProjectAsync(Id);
        await _projectCacheHelper.RemoveInstance(Id);
        return await task;
    }

    public async Task<IEnumerable<IProject>> GetPagedProjectsAsync(int pageIndex, int pageSize)
    {
        IEnumerable<IProject>? cachedResult = await _projectCacheHelper.GetCollection(pageIndex, pageSize);
        if (cachedResult is not null)
        {
            return cachedResult;
        }

        IEnumerable<IProject> result = await _projectManager.GetPagedProjectsAsync(pageIndex, pageSize);
        await _projectCacheHelper.SetCollection(result, pageIndex, pageSize);
        return result;
    }

    public async Task<IProject?> GetProjectAsync(int Id)
    {
        IProject? cachedResult = await _projectCacheHelper.GetInstance(Id);
        if (cachedResult is not null)
        {
            return cachedResult;
        }

        IProject? project = await _projectManager.GetProjectAsync(Id);
        if (project is not null)
        {
            await _projectCacheHelper.SetInstance(project);
        }

        return project;
    }

    public async Task<IEnumerable<IProject>> GetProjectsAsync()
    {
        IEnumerable<IProject>? cachedResult = await _projectCacheHelper.GetCollectionAllProjects();
        if (cachedResult is not null)
        {
            return cachedResult;
        }

        IEnumerable<IProject> result = await _projectManager.GetProjectsAsync();
        await _projectCacheHelper.SetCollection(result);
        return result;
    }

    public async Task<bool> UpdateProjectAsync(int Id, IProjectDTO projectDTO)
    {
        Task<bool> task = _projectManager.UpdateProjectAsync(Id, projectDTO);
        await _projectCacheHelper.RemoveInstance(Id);
        return await task;
    }
}
