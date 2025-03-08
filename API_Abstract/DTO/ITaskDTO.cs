namespace API_Abstract.DTO;
public interface ITaskDTO
{
    string title { get; }
    string? description { get; }
    bool isCompleted { get; }
    int projectId { get; }
}
