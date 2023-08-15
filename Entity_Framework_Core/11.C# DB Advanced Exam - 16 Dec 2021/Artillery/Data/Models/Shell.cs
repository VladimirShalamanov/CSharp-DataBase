namespace Artillery.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Shell
    {
        [Key]
        public int Id { get; set; }

        public double ShellWeight { get; set; }

        [Required]
        public string Caliber { get; set; } = null!;

        public ICollection<Gun> Guns { get; set; } = new HashSet<Gun>();
    }
}
