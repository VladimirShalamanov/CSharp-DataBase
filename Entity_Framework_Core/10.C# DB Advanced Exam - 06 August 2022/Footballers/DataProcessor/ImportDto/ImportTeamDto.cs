namespace Footballers.DataProcessor.ImportDto
{
    using System.ComponentModel.DataAnnotations;

    partial class ImportTeamDto
    {
        [Required]
        [MinLength(3)]
        [MaxLength(40)]
        [RegularExpression(@"^[A-Za-z0-9\s\.\-]{3,}$")]
        public string Name { get; set; } = null!;

        [Required]
        [MinLength(2)]
        [MaxLength(40)]
        public string Nationality { get; set; } = null!;

        [Required]
        public int Trophies { get; set; }

        public int[] Footballers { get; set; } = null!;
    }
}
