using API_Abstract.POCO;

namespace API_Abstract.Managers;
public interface IProjectManager
{
    Task<IEnumerable<IProjectPOCO>> GetProjectsAsync();
    Task<IEnumerable<IProjectPOCO>> GetPagedProjectsAsync(int pageIndex, int pageSize);
    Task<IProjectPOCO?> GetProjectAsync(int Id);
    Task<IProjectPOCO?> CreateProjectAsync(string name, string? description);
    Task<IProjectPOCO?> UpdateProjectAsync(int Id, string? name, string? description);
    Task<bool> DeleteProjectAsync(int Id);
}
