using API_Abstract.DTO;

namespace Web_API.Models;
public class ProjectDTO : IProjectDTO
{
    public string name { get; set; } = string.Empty;

    public string? description { get; set; }
}
