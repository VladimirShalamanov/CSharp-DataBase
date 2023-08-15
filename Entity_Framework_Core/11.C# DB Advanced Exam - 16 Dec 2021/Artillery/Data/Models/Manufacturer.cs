namespace Artillery.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Manufacturer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ManufacturerName { get; set; } = null!;

        [Required]
        public string Founded { get; set; } = null!;

        public ICollection<Gun> Guns { get; set; } = new HashSet<Gun>();
    }
}
