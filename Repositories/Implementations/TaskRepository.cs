namespace FlowAuthTasks.API.Repositories.Implementations
{
    using FlowAuthTasks.API.Data;
    using FlowAuthTasks.API.Models;
    using FlowAuthTasks.API.Repositories.Interfaces;
    using Microsoft.EntityFrameworkCore;
    using System.Linq;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class TaskRepository : ITaskRepository
    {
        private readonly ApplicationDbContext _context;

        public TaskRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<TaskItem> AddAsync(TaskItem task)
        {
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
            return task;
        }

        public async Task<TaskItem?> GetByIdAsync(int id)
        {
            return await _context.Tasks.FindAsync(id);
        }

        public async Task<IEnumerable<TaskItem>> GetByUserIdAsync(string userId)
        {
            return await _context.Tasks
                .Where(t => t.AssignedToUserId == userId)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
