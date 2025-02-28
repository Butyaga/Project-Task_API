namespace API_Abstract;
public interface IProject
{
    int Id { get; }
    string Name { get; }
    string? Description { get; }
    DateTime CreatedAt { get; }
    IEnumerable<ITask> Tasks { get; }
}
