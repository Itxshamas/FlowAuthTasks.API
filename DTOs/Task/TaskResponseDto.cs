namespace FlowAuthTasks.API.DTOs.Task
{
    public class TaskResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Status { get; set; } = "Pending";
        public DateTime CreatedAt { get; set; }
        public string? AssignedToUserId { get; set; }
    }
}
