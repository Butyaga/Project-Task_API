using API_Abstract.POCO;

namespace CacheRedis.Models;
class ProjectDTO : IProject
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public IEnumerable<ProjectTaskDTO> Tasks { get; set; } = [];

    IEnumerable<ITask> IProject.Tasks => Tasks;
}
