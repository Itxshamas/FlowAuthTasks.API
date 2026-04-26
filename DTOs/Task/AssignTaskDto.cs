namespace FlowAuthTasks.API.DTOs.Task
{
    public class AssignTaskDto
    {
        public int TaskId { get; set; }
        public string UserId { get; set; } = string.Empty;
    }
}
