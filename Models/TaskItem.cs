namespace FlowAuthTasks.API.Models
{
    public class TaskItem
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string Status { get; set; } = "Pending";
        // Pending, InProgress, Completed

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // 🔗 Foreign Key
        public string? AssignedToUserId { get; set; }

        // 🔗 Navigation Property
        public ApplicationUser? AssignedToUser { get; set; }
    }
}