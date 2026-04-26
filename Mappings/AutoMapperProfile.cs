namespace FlowAuthTasks.API.Mappings
{
    using AutoMapper;
    using FlowAuthTasks.API.DTOs.Task;
    using FlowAuthTasks.API.Models;

    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<CreateTaskDto, TaskItem>();
            CreateMap<TaskItem, TaskResponseDto>();
        }
    }
}
