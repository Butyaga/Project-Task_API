using API_Abstract.POCO;
using API_Abstract.Managers;
using Microsoft.AspNetCore.Mvc;
using Web_API.Models;

namespace Web_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController(ITaskManager _taskManager) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ITaskPOCO>>> GetTasksAsync([FromQuery] bool? isCompleted, [FromQuery] int? projectId)
        {
            if (projectId.HasValue && projectId.Value < 0)
            {
                return BadRequest(new Message("Wrong parameter values: projectId"));
            }

            try
            {
                IEnumerable<ITaskPOCO> tasks = await _taskManager.GetTasksAsync(isCompleted, projectId);
                return Ok(tasks);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ITaskPOCO>> GetTaskAsync(int id)
        {
            if (id < 0)
            {
                return BadRequest(new Message("Wrong parameter values: id"));
            }

            try
            {
                ITaskPOCO? task = await _taskManager.GetTaskAsync(id);
                if (task is null)
                {
                    return NotFound(new { message = $"No task found with Id {id}" });
                }

                return Ok(task);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<ActionResult<ITaskPOCO>> Post([FromBody] TaskDTO taskDTO)
        {
            // Check model
            //
            //

            try
            {
                ITaskPOCO result = await _taskManager.CreateTaskAsync(taskDTO);
                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] TaskDTO taskDTO)
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
                bool result = await _taskManager.UpdateTaskAsync(id, taskDTO);
                if (result)
                {
                    return NoContent();
                }
                return NotFound(new { message = $"No task found with Id {id}" });
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTaskAsync(int id)
        {
            if (id < 0)
            {
                return BadRequest(new Message("Wrong parameter values: id"));
            }

            try
            {
                bool result = await _taskManager.DeleteTaskAsync(id);
                if (result)
                {
                    return NoContent();
                }
                return NotFound(new { message = $"No task found with Id {id}" });
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
