using API_Abstract.DTO;
using API_Abstract.Managers;
using API_Abstract.POCO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheRedis.AbstractRealisation;
public class ProjectManagerProxy(IProjectManager _projectManager) : IProjectManagerProxy
{
    public async Task<IProject> CreateProjectAsync(IProjectDTO projectDTO)
    {
        return await _projectManager.CreateProjectAsync(projectDTO);
    }

    public async Task<bool> DeleteProjectAsync(int Id)
    {
        return await _projectManager.DeleteProjectAsync(Id);
    }

    public async Task<IEnumerable<IProject>> GetPagedProjectsAsync(int pageIndex, int pageSize)
    {
        return await _projectManager.GetPagedProjectsAsync(pageIndex, pageSize);
    }

    public async Task<IProject?> GetProjectAsync(int Id)
    {
        return await _projectManager.GetProjectAsync(Id);
    }

    public async Task<IEnumerable<IProject>> GetProjectsAsync()
    {
        return await _projectManager.GetProjectsAsync();
    }

    public async Task<bool> UpdateProjectAsync(int Id, IProjectDTO projectDTO)
    {
        return await _projectManager.UpdateProjectAsync(Id, projectDTO);
    }
}
