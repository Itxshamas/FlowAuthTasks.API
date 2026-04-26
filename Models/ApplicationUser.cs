using Microsoft.AspNetCore.Identity;

namespace FlowAuthTasks.API.Models
{
    public class ApplicationUser : IdentityUser
    {
        // You can extend later
        public string? FullName { get; set; }

        // Navigation Property
        public ICollection<TaskItem>? Tasks { get; set; }
    }
}