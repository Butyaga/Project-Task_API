﻿using API_Abstract.POCO;

namespace DB_Manager.Models.DTO;
public class ProjectDTO : IProject
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public IEnumerable<ITask> Tasks { get; set; } = [];
}
