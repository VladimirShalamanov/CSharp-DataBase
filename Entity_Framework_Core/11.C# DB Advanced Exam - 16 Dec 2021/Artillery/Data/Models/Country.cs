namespace Artillery.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Country
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string CountryName { get; set; } = null!;

        public int ArmySize { get; set; }

        public ICollection<CountryGun> CountriesGuns { get; set; } = new HashSet<CountryGun>();
    }
}
