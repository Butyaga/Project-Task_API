using API_Abstract.DTO;
using API_Abstract.Managers;
using API_Abstract.POCO;
using DB_Manager.DBCntxt;
using DB_Manager.Models.DTO;
using DB_Manager.Models;
using Microsoft.EntityFrameworkCore;

namespace DB_Manager.Managers;
public class TaskManager(PgSQLContext _context) : ITaskManager
{
    public async Task<ITask> CreateTaskAsync(ITaskDTO taskDTO)
    {
        ProjectTask newTask = new()
        {
            Title = taskDTO.title,
            Description = taskDTO.description,
            IsCompleted = taskDTO.isCompleted,
            ProjectId = taskDTO.projectId
        };

        await _context.ProjectTasks.AddAsync(newTask);
        int result = await _context.SaveChangesAsync();
        if (result == 0)
        {
            throw new Exception("Task creation error in DB");
        }
        return newTask.MapToDTO();
    }

    public async Task<bool> DeleteTaskAsync(int id)
    {
        ProjectTask? task = await _context.ProjectTasks.FirstOrDefaultAsync(t => t.Id == id);

        if (task is null)
        {
            return false;
        }

        _context.ProjectTasks.Remove(task);
        int result = await _context.SaveChangesAsync();
        if (result == 0)
        {
            throw new Exception("Task creation error in DB");
        }
        return true;
    }

    public async Task<ITask?> GetTaskAsync(int id)
    {
        ProjectTask? task = await _context.ProjectTasks.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);

        if (task is null)
        {
            return null;
        }

        return task.MapToDTO();
    }

    public async Task<IEnumerable<ITask>> GetTasksAsync(bool? isCompleted, int? projectId)
    {
        IQueryable<ProjectTask> queryTask = GetFilteredQuery(isCompleted, projectId);
        List <ProjectTask> tasks = await queryTask.ToListAsync();
        IEnumerable<ITask> tasksDTO = [.. from task in tasks select task.MapToDTO()];
        return tasksDTO;
    }

    public async Task<bool> UpdateTaskAsync(int id, ITaskDTO taskDTO)
    {
        ProjectTask? task = await _context.ProjectTasks.FirstOrDefaultAsync(t => t.Id == id);
        if (task is null)
        {
            return false;
        }

        task.Title = taskDTO.title;
        task.Description = taskDTO.description;
        task.IsCompleted = taskDTO.isCompleted;
        task.ProjectId = taskDTO.projectId;

        int result = await _context.SaveChangesAsync();
        if (result == 0)
        {
            throw new Exception("Task update error in DB");
        }
        return true;
    }

    private IQueryable<ProjectTask> GetFilteredQuery(bool? isCompleted, int? projectId)
    {
        IQueryable<ProjectTask> query = _context.ProjectTasks.AsNoTracking();
        if (projectId.HasValue)
        {
            query = query.Where(task => task.ProjectId == projectId.Value);
        }
        if (isCompleted.HasValue)
        {
            query = query.Where(task => task.IsCompleted == isCompleted.Value);
        }
        return query;
    }
}
