namespace API_Abstract;
public interface IProjectManager
{
    Task<IEnumerable<IProject>> GetProjectsAsync();
    Task<IEnumerable<IProject>> GetPagedProjectsAsync(int pageIndex, int pageSize);
    Task<IProject?> GetProjectAsync(int Id);
    Task<IProject> CreateProjectAsync(string name, string? description);
    Task<IProject?> UpdateProjectAsync(int Id, string? name, string? description);
    Task<bool> DeleteProjectAsync(int Id);
}
