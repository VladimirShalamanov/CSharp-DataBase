namespace Artillery.Data.Models
{
    using System.ComponentModel.DataAnnotations.Schema;

    public class CountryGun
    {
        [ForeignKey("Country")]
        public int CountryId { get; set; }
        public Country Country { get; set; } = null!;


        [ForeignKey("Gun")]
        public int GunId { get; set; }
        public Gun Gun { get; set; } = null!;
    }
}
