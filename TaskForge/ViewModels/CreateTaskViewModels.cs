using System.ComponentModel.DataAnnotations;
using TaskForge.Models;

namespace TaskForge.ViewModels
{
    public class CreateTaskViewModels
    {
        [Required, MaxLength(200)]
        public string Title { get; set; } = string.Empty;


        [MaxLength(1000)]
        public string? Description { get; set; }

        public DateTime? DueDate { get; set; }

        public TaskForge.Models.TaskStatus Status { get; set; } = TaskForge.Models.TaskStatus.ToDo;

        public int ProjectId { get; set; }

        public string AssignedToUserId { get; set; } = string.Empty;
    }

}
