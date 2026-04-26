namespace FlowAuthTasks.API.Services.Interfaces
{
    using FlowAuthTasks.API.DTOs.Task;
    using FlowAuthTasks.API.Helpers;

    public interface ITaskService
    {
        Task<ApiResponse<TaskResponseDto>> CreateTaskAsync(CreateTaskDto dto);
        Task<ApiResponse<string>> AssignTaskAsync(AssignTaskDto dto);
        Task<ApiResponse<IEnumerable<TaskResponseDto>>> GetMyTasksAsync(string userId);
    }
}
