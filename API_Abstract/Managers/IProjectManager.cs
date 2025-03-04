using API_Abstract.DTO;
using API_Abstract.POCO;

namespace API_Abstract.Managers;
public interface IProjectManager
{
    Task<IEnumerable<IProjectPOCO>> GetProjectsAsync();
    Task<IEnumerable<IProjectPOCO>> GetPagedProjectsAsync(int pageIndex, int pageSize);
    Task<IProjectPOCO?> GetProjectAsync(int Id);
    Task<IProjectPOCO> CreateProjectAsync(IProjectDTO projectDTO);
    Task<bool> UpdateProjectAsync(int Id, IProjectDTO projectDTO);
    Task<bool> DeleteProjectAsync(int Id);
}
