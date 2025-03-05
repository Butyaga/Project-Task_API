﻿namespace DB_Manager.Models;
public class Project
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public List<ProjectTask> Tasks { get; set; } = [];
}
