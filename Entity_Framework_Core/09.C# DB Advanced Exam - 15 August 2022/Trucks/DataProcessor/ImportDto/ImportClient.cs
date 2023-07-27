namespace Trucks.DataProcessor.ImportDto
{
    using System.ComponentModel.DataAnnotations;
    using SettingsProp;

    public class ImportClient
    {
        [Required]
        [MinLength(Constants.ClientNameMinLength)]
        [MaxLength(Constants.ClientNameMaxLength)]
        public string Name { get; set; } = null!;

        [Required]
        [MinLength(Constants.ClientNationalityMinLength)]
        [MaxLength(Constants.ClientNationalityMaxLength)]
        public string Nationality { get; set; } = null!;

        [Required]
        public string Type { get; set; } = null!;

        public int[] Trucks { get; set; }
    }
}
