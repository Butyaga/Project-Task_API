namespace API_Abstract.POCO;
public interface ITask
{
    int Id { get; }
    string Title { get; }
    string? Description { get; }
    bool IsCompleted { get; }
    DateTime CreatedAt { get; }
    DateTime UpdatedAt { get; }
    int ProjectId { get; }
}
