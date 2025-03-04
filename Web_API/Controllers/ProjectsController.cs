using API_Abstract.POCO;
using API_Abstract.Managers;
using Microsoft.AspNetCore.Mvc;
using Web_API.Models;

namespace Web_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectsController(IProjectManager _projectManager) : ControllerBase
{
    [HttpGet()]
    public async Task<ActionResult<IEnumerable<IProjectPOCO>>> GetProjectsAsync([FromQuery] int page = 0, [FromQuery] int pageSize = 0)
    {
        if (pageSize < 0 || page < -1)
        {
            return BadRequest(new Message("Wrong parameter values: page, pageSize"));
        }

        try
        {
            IEnumerable<IProjectPOCO> projects;
            if (pageSize == 0)
            {
                projects = await _projectManager.GetProjectsAsync();
            }
            else
            {
                projects = await _projectManager.GetPagedProjectsAsync(page, pageSize);
            }
            return Ok(projects);
        }
        catch (Exception)
        {
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<IProjectPOCO>> GetProjectAsync(int id)
    {
        if (id < 1)
        {
            return BadRequest(new Message("Wrong parameters value: id"));
        }

        try
        {
            IProjectPOCO? project = await _projectManager.GetProjectAsync(id);
            if (project is null)
            {
                return NotFound(new Message($"No project found with Id {id}"));
            }
            return Ok(project);
        }
        catch (Exception)
        {
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost]
    public async Task<ActionResult<IProjectPOCO>> PostProjectAsync([FromBody] ProjectDTO project)
    {
        // Check model
        //
        //

        try
        {
            IProjectPOCO result = await _projectManager.CreateProjectAsync(project);
            return CreatedAtAction(nameof(GetProjectAsync), result.Id, result);
        }
        catch (Exception)
        {
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> PutProjectAsync(int id, [FromBody] ProjectDTO project)
    {
        if (id < 0)
        {
            return BadRequest(new Message("Wrong parameter values: id"));
        }

        // Check model
        //
        //

        try
        {
            bool result = await _projectManager.UpdateProjectAsync(id, project);
            if (result)
            {
                return NoContent();
            }
            return NotFound(new Message($"No project found with Id {id}"));
        }
        catch (Exception)
        {
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteProjectAsync(int id)
    {
        if (id < 0)
        {
            return BadRequest(new Message("Wrong parameter values: id"));
        }

        try
        {
            bool result = await _projectManager.DeleteProjectAsync(id);
            if (result)
            {
                return Ok();
            }
            return NotFound(new Message($"No project found with Id {id}"));
        }
        catch (Exception)
        {
            return StatusCode(500, "Internal server error");
        }
    }
}
