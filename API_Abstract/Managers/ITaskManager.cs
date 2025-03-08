using API_Abstract.DTO;
using API_Abstract.POCO;

namespace API_Abstract.Managers;
public interface ITaskManager
{
    Task<IEnumerable<ITask>> GetTasksAsync(bool? isCompleted, int? projectId);
    Task<ITask?> GetTaskAsync(int id);
    Task<ITask> CreateTaskAsync(ITaskDTO taskDTO);
    Task<bool> UpdateTaskAsync(int id, ITaskDTO taskDTO);
    Task<bool> DeleteTaskAsync(int id);
}
