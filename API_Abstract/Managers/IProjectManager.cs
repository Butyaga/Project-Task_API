using API_Abstract.DTO;
using API_Abstract.POCO;

namespace API_Abstract.Managers;
public interface IProjectManager
{
    Task<IEnumerable<IProject>> GetProjectsAsync();
    Task<IEnumerable<IProject>> GetPagedProjectsAsync(int pageIndex, int pageSize);
    Task<IProject?> GetProjectAsync(int Id);
    Task<IProject> CreateProjectAsync(IProjectDTO projectDTO);
    Task<bool> UpdateProjectAsync(int Id, IProjectDTO projectDTO);
    Task<bool> DeleteProjectAsync(int Id);
}
