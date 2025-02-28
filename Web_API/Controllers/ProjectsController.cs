using API_Abstract;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using Web_API.Models;

namespace Web_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectsController(IProjectManager _projectManager) : ControllerBase
{
    [HttpGet()]
    public async Task<IResult> GetProgectsAsync([FromQuery]int page = 0, [FromQuery]int pageSize = 0)
    {
        if (pageSize < 0 || page < -1)
        {
            return Results.BadRequest("Wrong parameter values: page, pageSize");
        }

        IEnumerable<IProject> projects;
        if (pageSize == 0)
        {
            projects = await _projectManager.GetProjectsAsync();
        } else
        {
            projects = await _projectManager.GetPagedProjectsAsync(page, pageSize);
        }
        return Results.Ok(projects);
    }

    [HttpGet("{id}")]
    public async Task<IResult> GetProgectAsync(int id)
    {
        if (id < 1)
        {
            return Results.BadRequest("Wrong parameters value: id");
        }

        IProject? project = await _projectManager.GetProjectAsync(id);
        if (project is null)
        {
            return Results.BadRequest("Wrong project Id");
        }
        return Results.Ok(project);
    }

    [HttpPost]
    public async Task<IResult> PostProjectAsync([FromBody]string name, [FromBody]string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Results.UnprocessableEntity("Field name is null or empty");
        }

        IProject project = await _projectManager.CreateProjectAsync(name, description);
        return Results.Created($"/api/projects/{project.Id}", project);
    }

    [HttpPut("{id}")]
    public async Task<IResult> PutProjectAsync(int id, [FromBody] string? name, [FromBody] string? description)
    {
        if (!IsValidUpdateEntity(name, description))
        {
            return Results.UnprocessableEntity("Wrong fields value for update");
        }

        IProject? project = await _projectManager.UpdateProjectAsync(id, name, description);
        if (project is null)
        {
            return Results.BadRequest("Wrong project Id");
        }
        return Results.Ok(project);
    }

    [HttpDelete("{id}")]
    public async Task<IResult> DeleteProjectAsync(int id)
    {
        if (id < 1)
        {
            return Results.BadRequest("Wrong parameters value: id");
        }

        bool deleteResult = await _projectManager.DeleteProjectAsync(id);
        if (deleteResult)
        {
            return Results.Ok();
        }
        return Results.NotFound("Not found project id");
    }

    private static bool IsValidUpdateEntity(string? name, string? description)
    {
        // Пустое поле имени проекта
        if (name?.Trim() == "")
        {
            return false;
        }

        // Отсутствие полей для обновления
        if (name is null && description is null)
        {
            return false;
        }

        return true;
    }
}
