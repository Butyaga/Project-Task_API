using API_Abstract;

namespace Web_API.Models;
public class Project : IProject
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public IEnumerable<ITask> Tasks { get; set; } = [];
}
