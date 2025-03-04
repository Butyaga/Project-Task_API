namespace API_Abstract.POCO;
public interface IProjectPOCO
{
    int Id { get; }
    string Name { get; }
    string? Description { get; }
    DateTime CreatedAt { get; }
    IEnumerable<ITaskPOCO> Tasks { get; }
}
