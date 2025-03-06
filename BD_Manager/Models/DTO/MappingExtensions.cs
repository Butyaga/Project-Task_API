namespace DB_Manager.Models.DTO;
static class MappingExtensions
{
    public static ProjectTaskDTO MapToDTO(this ProjectTask? mappingObject)
    {
        ArgumentNullException.ThrowIfNull(mappingObject);

        ProjectTaskDTO dto = new()
        {
            Id = mappingObject.Id,
            Title = mappingObject.Title,
            Description = mappingObject.Description,
            IsCompleted = mappingObject.IsCompleted,
            CreatedAt = mappingObject.CreatedAt,
            UpdatedAt = mappingObject.UpdatedAt,
            ProjectId = mappingObject.ProjectId
        };
        return dto;
    }

    public static ProjectDTO MapToDTO(this Project? mappingObject)
    {
        ArgumentNullException.ThrowIfNull(mappingObject);

        ProjectDTO dto = new()
        {
            Id = mappingObject.Id,
            Name = mappingObject.Name,
            Description = mappingObject.Description,
            CreatedAt = mappingObject.CreatedAt,
            Tasks = [.. from task in mappingObject.Tasks select task.MapToDTO()]
        };
        return dto;
    }
}
