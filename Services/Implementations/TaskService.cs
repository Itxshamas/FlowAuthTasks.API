namespace FlowAuthTasks.API.Services.Implementations
{
    using AutoMapper;
    using FlowAuthTasks.API.DTOs.Task;
    using FlowAuthTasks.API.Helpers;
    using FlowAuthTasks.API.Models;
    using FlowAuthTasks.API.Repositories.Interfaces;
    using FlowAuthTasks.API.Services.Interfaces;

    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _repo;
        private readonly IMapper _mapper;

        public TaskService(ITaskRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<ApiResponse<TaskResponseDto>> CreateTaskAsync(CreateTaskDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Title))
                return ApiResponse<TaskResponseDto>.CreateFail("Title is required");

            var entity = _mapper.Map<FlowAuthTasks.API.Models.TaskItem>(dto);
            var created = await _repo.AddAsync(entity);
            var response = _mapper.Map<TaskResponseDto>(created);
            return ApiResponse<TaskResponseDto>.CreateSuccess(response, "Task created");
        }

        public async Task<ApiResponse<string>> AssignTaskAsync(AssignTaskDto dto)
        {
            var task = await _repo.GetByIdAsync(dto.TaskId);
            if (task is null)
                return ApiResponse<string>.CreateFail("Task not found");

            task.AssignedToUserId = dto.UserId;
            await _repo.SaveChangesAsync();
            return ApiResponse<string>.CreateSuccess(string.Empty, "Task assigned");
        }

        public async Task<ApiResponse<IEnumerable<TaskResponseDto>>> GetMyTasksAsync(string userId)
        {
            var tasks = await _repo.GetByUserIdAsync(userId);
            var dtos = tasks.Select(t => _mapper.Map<TaskResponseDto>(t));
            return ApiResponse<IEnumerable<TaskResponseDto>>.CreateSuccess(dtos, "OK");
        }
    }
}
