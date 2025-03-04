namespace API_Abstract.POCO;
public interface ITaskPOCO
{
    int Id { get; }
    string Title { get; }
    string? Description { get; }
    bool IsCompleted { get; }
    DateTime CreatedAt { get; }
    DateTime UpdatedAt { get; }
    int ProjectId { get; }
    //Project(навигационное свойство для связи с проектом, тип Project).
}
