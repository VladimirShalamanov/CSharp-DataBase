namespace Boardgames.DataProcessor.ImportDto
{
    using System.ComponentModel.DataAnnotations;

    public class ImportSellerDto
    {
        [Required]
        [MinLength(5)]
        [MaxLength(20)]
        public string Name { get; set; } = null!;

        [Required]
        [MinLength(2)]
        [MaxLength(30)]
        public string Address { get; set; } = null!;

        [Required]
        public string Country { get; set; } = null!;

        [RegularExpression(@"w{3}\.[a-zA-Z0-9\-]+\.[com]+")]
        public string? Website { get; set; }

        public int[] Boardgames { get; set; } = null!;
    }
}
