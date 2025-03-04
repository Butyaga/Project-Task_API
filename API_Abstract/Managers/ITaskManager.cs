using API_Abstract.DTO;
using API_Abstract.POCO;

namespace API_Abstract.Managers;
public interface ITaskManager
{
    Task<IEnumerable<ITaskPOCO>> GetTasksAsync(bool? isCompleted, int? projectId);
    Task<ITaskPOCO?> GetTaskAsync(int id);
    Task<ITaskPOCO> CreateTaskAsync(ITaskDTO taskDTO);
    Task<bool> UpdateTaskAsync(int id, ITaskDTO taskDTO);
    Task<bool> DeleteTaskAsync(int id);
}
