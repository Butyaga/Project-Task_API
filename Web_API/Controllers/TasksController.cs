using API_Abstract.POCO;
using API_Abstract.Managers;
using Microsoft.AspNetCore.Mvc;
using Web_API.Models;
using FluentValidation;
using FluentValidation.Results;

namespace Web_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController(ITaskManager _taskManager, ILogger<ProjectsController> _logger) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ITask>>> GetTasks([FromQuery] bool? isCompleted, [FromQuery] int? projectId)
    {
        _logger.LogInformation("Запущен метод Get");
        _logger.LogDebug("Параметры в запроме метода: isCompleted = {isCompleted}, projectId = {projectId}", isCompleted, projectId);
        if (projectId.HasValue && projectId.Value < 0)
        {
            _logger.LogInformation("Значения параметров неприемлемые");
            return BadRequest(new Message("Wrong parameter values: projectId"));
        }

        try
        {
            _logger.LogInformation("Запрощены задачи с фильтрацией по полям isCompleted = {isCompleted}, projectId = {projectId}", isCompleted, projectId);
            IEnumerable<ITask> tasks = await _taskManager.GetTasksAsync(isCompleted, projectId);
            _logger.LogInformation("Успешное запершение запроса");
            _logger.LogDebug("Возращаемое значение списка: {@tasks}", tasks);
            return Ok(tasks);
        }
        catch (Exception ex )
        {
            _logger.LogError(ex, "Что-то пошло не так");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ITask>> GetTask(int id)
    {
        _logger.LogInformation("Запущен метод Get");
        _logger.LogDebug("Параметры в запроме метода: id = {id}", id);
        if (id < 0)
        {
            _logger.LogInformation("Значения параметров неприемлемые");
            return BadRequest(new Message("Wrong parameter values: id"));
        }

        try
        {
            _logger.LogInformation("Запрощена задача с Id = {id}", id);
            ITask? task = await _taskManager.GetTaskAsync(id);
            if (task is null)
            {
                _logger.LogInformation("Задача с Id = {id} не найдена", id);
                return NotFound(new Message($"No task found with Id {id}"));
            }
            _logger.LogInformation("Успешное запершение запроса");
            _logger.LogDebug("Возращаемое значение: {@task}", task);
            return Ok(task);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Что-то пошло не так");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost]
    public async Task<ActionResult<ITask>> PostTask([FromBody] TaskDTO taskDTO, [FromServices] IValidator<TaskDTO> validator)
    {
        _logger.LogInformation("Запущен метод Post");
        _logger.LogDebug("В теле запроса предана сущность: {@taskDTO}", taskDTO);
        ValidationResult validationResult = validator.Validate(taskDTO);
        if (!validationResult.IsValid)
        {
            _logger.LogInformation("Сущность TaskDTO не валидна");
            return StatusCode(422, "TaskDTO is not valid");
        }

        try
        {
            _logger.LogInformation("Запрощено создание объекта");
            ITask result = await _taskManager.CreateTaskAsync(taskDTO);
            _logger.LogInformation("Успешное запершение запроса");
            return CreatedAtAction(nameof(GetTask), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Что-то пошло не так");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> PutTask(int id, [FromBody] TaskDTO taskDTO, [FromServices] IValidator<TaskDTO> validator)
    {
        _logger.LogInformation("Запущен метод Put");
        _logger.LogDebug("В запрос преданы: id = {id}, TaskDTO = {@taskDTO}", id, taskDTO);
        if (id < 0)
        {
            _logger.LogInformation("Значения параметра id неприемлемо");
            return BadRequest(new Message("Wrong parameter values: id"));
        }

        ValidationResult validationResult = validator.Validate(taskDTO);
        if (!validationResult.IsValid)
        {
            _logger.LogInformation("Сущность ProjectDTO не валидна");
            return StatusCode(422, "TaskDTO is not valid");
        }

        try
        {
            _logger.LogInformation("Запрощено обновление объекта");
            bool result = await _taskManager.UpdateTaskAsync(id, taskDTO);
            if (result)
            {
                _logger.LogInformation("Успешное запершение запроса");
                return NoContent();
            }
            _logger.LogInformation("Объект для обновления не найден");
            return NotFound(new Message($"No task found with Id {id}"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Что-то пошло не так");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteTask(int id)
    {
        _logger.LogInformation("Запущен метод Delete");
        _logger.LogDebug("В запрос преданы: id = {id}", id);
        if (id < 0)
        {
            _logger.LogInformation("Значения параметра id неприемлемо");
            return BadRequest(new Message("Wrong parameter values: id"));
        }

        try
        {
            _logger.LogInformation("Запрощено удаление объекта");
            bool result = await _taskManager.DeleteTaskAsync(id);
            if (result)
            {
                _logger.LogInformation("Объект удален");
                return Ok();
            }
            _logger.LogInformation("Объект для удаления не найден");
            return NotFound(new Message($"No task found with Id {id}"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Что-то пошло не так");
            return StatusCode(500, "Internal server error");
        }
    }
}
