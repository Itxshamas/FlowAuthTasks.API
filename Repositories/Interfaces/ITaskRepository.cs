using System.Collections.Generic;
using System.Threading.Tasks;
using FlowAuthTasks.API.Models;

namespace FlowAuthTasks.API.Repositories.Interfaces
{
    public interface ITaskRepository
    {
        Task<TaskItem> AddAsync(TaskItem task);
        Task<TaskItem?> GetByIdAsync(int id);
        Task<IEnumerable<TaskItem>> GetByUserIdAsync(string userId);
        Task SaveChangesAsync();
    }
}
