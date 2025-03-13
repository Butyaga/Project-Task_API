using API_Abstract.POCO;
using API_Abstract.Managers;
using Microsoft.AspNetCore.Mvc;
using Web_API.Models;
using Web_API.Models.Validators;
using FluentValidation.Results;
using FluentValidation;
using CacheRedis.AbstractRealisation;

namespace Web_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectsController(IProjectManagerProxy projectManagerProxy, ILogger<ProjectsController> _logger) : ControllerBase
{
    private readonly IProjectManager _projectManager = projectManagerProxy;

    [HttpGet()]
    public async Task<ActionResult<IEnumerable<IProject>>> GetProjects([FromQuery] int page = 0, [FromQuery] int pageSize = 0)
    {
        _logger.LogInformation("Запущен метод Get");
        _logger.LogDebug("Параметры в запроме метода: page = {page}, pageSize = {pageSize}", page, pageSize);
        if (pageSize < 0 || page < 0)
        {
            _logger.LogInformation("Значения параметров неприемлемые");
            return BadRequest(new Message("Wrong parameter values: page, pageSize"));
        }

        try
        {
            IEnumerable<IProject> projects;
            if (pageSize == 0)
            {
                _logger.LogInformation("Запрощен полный список проектов");
                projects = await _projectManager.GetProjectsAsync();
            }
            else
            {
                _logger.LogInformation("Запрощена страница № {page}, размер страницы {pageSize}", page, pageSize);
                projects = await _projectManager.GetPagedProjectsAsync(page, pageSize);
            }
            _logger.LogInformation("Успешное запершение запроса");
            _logger.LogDebug("Возращаемое значение списка: {@projects}", projects);
            return Ok(projects);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Что-то пошло не так");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<IProject>> GetProject(int id)
    {
        _logger.LogInformation("Запущен метод Get");
        _logger.LogDebug("Параметры в запроме метода: id = {id}", id);
        if (id < 1)
        {
            _logger.LogInformation("Значения параметров неприемлемые");
            return BadRequest(new Message("Wrong parameters value: id"));
        }

        try
        {
            _logger.LogInformation("Запрощен проект с Id = {id}", id);
            IProject? project = await _projectManager.GetProjectAsync(id);
            if (project is null)
            {
                _logger.LogInformation("Проект с Id = {id} не найден", id);
                return NotFound(new Message($"No project found with Id {id}"));
            }
            _logger.LogInformation("Успешное запершение запроса");
            _logger.LogDebug("Возращаемое значение: {@project}", project);
            return Ok(project);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Что-то пошло не так");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost]
    public async Task<ActionResult<IProject>> PostProject([FromBody] ProjectDTO project, [FromServices] IValidator<ProjectDTO> validator)
    {
        _logger.LogInformation("Запущен метод Post");
        _logger.LogDebug("В теле запроса предана сущность: {@project}", project);

        ValidationResult validationResult = await validator.ValidateAsync(project);
        if (!validationResult.IsValid)
        {
            _logger.LogInformation("Сущность ProjectDTO не валидна");
            validationResult.AddToModelState(ModelState);
            return BadRequest(ModelState);
        }

        try
        {
            _logger.LogInformation("Запрощено создание объекта");
            IProject result = await _projectManager.CreateProjectAsync(project);
            _logger.LogInformation("Успешное запершение запроса");
            return CreatedAtAction(nameof(GetProject), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Что-то пошло не так");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> PutProject(int id, [FromBody] ProjectDTO project, [FromServices] IValidator<ProjectDTO> validator)
    {
        _logger.LogInformation("Запущен метод Put");
        _logger.LogDebug("В запрос преданы: id = {id}, ProjectDTO = {@project}", id, project);

        if (id < 1)
        {
            _logger.LogInformation("Значения параметра id неприемлемо");
            return BadRequest(new Message("Wrong parameter values: id"));
        }

        ValidationResult validationResult = await validator.ValidateAsync(project);
        if (!validationResult.IsValid)
        {
            _logger.LogInformation("Сущность ProjectDTO не валидна");
            validationResult.AddToModelState(ModelState);
            return BadRequest(ModelState);
        }

        try
        {
            _logger.LogInformation("Запрощено обновление объекта");
            bool result = await _projectManager.UpdateProjectAsync(id, project);
            if (result)
            {
                _logger.LogInformation("Успешное запершение запроса");
                return NoContent();
            }
            _logger.LogInformation("Объект для обновления не найден");
            return NotFound(new Message($"No project found with Id {id}"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Что-то пошло не так");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteProject(int id)
    {
        _logger.LogInformation("Запущен метод Delete");
        _logger.LogDebug("В запрос преданы: id = {id}", id);

        if (id < 1)
        {
            _logger.LogInformation("Значения параметра id неприемлемо");
            return BadRequest(new Message("Wrong parameter values: id"));
        }

        try
        {
            _logger.LogInformation("Запрощено удаление объекта");
            bool result = await _projectManager.DeleteProjectAsync(id);
            if (result)
            {
                _logger.LogInformation("Объект удален");
                return Ok();
            }
            _logger.LogInformation("Объект для удаления не найден");
            return NotFound(new Message($"No project found with Id {id}"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Что-то пошло не так");
            return StatusCode(500, "Internal server error");
        }
    }
}
