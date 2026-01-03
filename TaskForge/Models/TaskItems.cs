using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace TaskForge.Models
{
    public class TaskItems
    {
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required]
        public TaskStatus Status { get; set; } = TaskStatus.ToDo;

        public DateTime? DueDate { get; set; }

        public int ProjectId { get; set; }
        public Project Project { get; set; } = null!;

        public string? AssignedToUserId { get; set; }
        public IdentityUser? AssignedUser { get; set; }

    }

    public enum TaskStatus
    {
        ToDo,
        InProgress,
        Done
    }
}
