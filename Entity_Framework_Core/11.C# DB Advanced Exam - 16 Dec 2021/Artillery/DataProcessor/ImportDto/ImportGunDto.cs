namespace Artillery.DataProcessor.ImportDto
{
    using Artillery.Data.Models.Enums;
    using Artillery.Data.Models;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.ComponentModel.DataAnnotations;

    public class ImportGunDto
    {
        public int ManufacturerId { get; set; }

        [Range(100, 1_350_000)]
        public int GunWeight { get; set; }

        [Range(2.00, 35.00)]
        public double BarrelLength { get; set; }

        public int? NumberBuild { get; set; }

        [Range(1, 100_000)]
        public int Range { get; set; }

        [Required]
        public string GunType { get; set; } = null!;

        public int ShellId { get; set; }

        public GunWithCountryDto[] Countries { get; set; } = null!;
    }
}
