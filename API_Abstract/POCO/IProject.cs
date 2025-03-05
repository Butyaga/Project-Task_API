namespace API_Abstract.POCO;
public interface IProject
{
    int Id { get; }
    string Name { get; }
    string? Description { get; }
    DateTime CreatedAt { get; }
    IEnumerable<ITask> Tasks { get; }
}
