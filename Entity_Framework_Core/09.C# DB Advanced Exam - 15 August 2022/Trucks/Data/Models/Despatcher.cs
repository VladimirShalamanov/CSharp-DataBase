namespace Trucks.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    using SettingsProp;

    public class Despatcher
    {
        public Despatcher()
        {
            this.Trucks = new HashSet<Truck>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        //[MaxLength(Constants.DespatcherNameMaxLength)]
        public string Name { get; set; } = null!;

        public string Position { get; set; }


        public ICollection<Truck> Trucks { get; set; }
    }
}
