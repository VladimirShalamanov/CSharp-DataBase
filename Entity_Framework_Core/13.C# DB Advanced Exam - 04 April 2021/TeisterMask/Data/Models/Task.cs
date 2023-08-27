namespace TeisterMask.Data.Models
{
    using Microsoft.VisualBasic;
    using System.ComponentModel.DataAnnotations;

    using Enums;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Task
    {
        public Task()
        {
            this.EmployeesTasks = new HashSet<EmployeeTask>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        public DateTime OpenDate { get; set; }

        public DateTime DueDate { get; set; }

        public ExecutionType ExecutionType { get; set; }

        public LabelType LabelType { get; set; }


        [Required]
        [ForeignKey(nameof(Project))]
        public int ProjectId { get; set; }
        public Project Project { get; set; } = null!;

        public ICollection<EmployeeTask> EmployeesTasks { get; set; } = null!;
    }
}
