using API_Abstract.DTO;

namespace Web_API.Models;
public class TaskDTO : ITaskDTO
{
    public string title { get; set; } = string.Empty;
    public string? description { get; set; }
    public bool isCompleted { get; set; }
    public int projectId { get; set; }
}
