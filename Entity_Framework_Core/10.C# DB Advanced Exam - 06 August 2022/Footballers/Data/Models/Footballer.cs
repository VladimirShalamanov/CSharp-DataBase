namespace Footballers.Data.Models
{
    using Footballers.Data.Models.Enums;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Footballer
    {
        public Footballer()
        {
            this.TeamsFootballers = new HashSet<TeamFootballer>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        public DateTime ContractStartDate { get; set; }

        public DateTime ContractEndDate { get; set; }

        [Required]
        public PositionType PositionType { get; set; }

        [Required]
        public BestSkillType BestSkillType { get; set; }

        [ForeignKey("CoachId")]
        public int CoachId { get; set; }
        public Coach Coach { get; set; } = null!;


        public ICollection<TeamFootballer> TeamsFootballers { get; set; }
    }
}
