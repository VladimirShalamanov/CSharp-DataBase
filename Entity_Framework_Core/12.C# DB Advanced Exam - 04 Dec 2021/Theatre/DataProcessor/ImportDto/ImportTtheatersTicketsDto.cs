namespace Theatre.DataProcessor.ImportDto
{
    using Newtonsoft.Json;
    using System.ComponentModel.DataAnnotations;
    using Theatre.Data.Models;

    public class ImportTtheatersTicketsDto
    {
        [Required]
        [MinLength(4)]
        [MaxLength(30)]
        [JsonProperty("Name")]
        public string Name { get; set; } = null!;

        [Range(1, 10)]
        [JsonProperty("NumberOfHalls")]
        public sbyte NumberOfHalls { get; set; }

        [Required]
        [MinLength(4)]
        [MaxLength(30)]
        [JsonProperty("Director")]
        public string Director { get; set; } = null!;

        [JsonProperty("Tickets")]
        public ImportTicketsDto[] Tickets { get; set; } = null!;
    }

    public class ImportTicketsDto
    {
        [Range(1.00, 100.00)]
        [JsonProperty("Price")]
        public decimal Price { get; set; }

        [Range(1, 10)]
        [JsonProperty("RowNumber")]
        public sbyte RowNumber { get; set; }

        [JsonProperty("PlayId")]
        public int PlayId { get; set; }
    }
}
