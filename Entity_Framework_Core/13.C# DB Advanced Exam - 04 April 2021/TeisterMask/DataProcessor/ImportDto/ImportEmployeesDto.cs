namespace TeisterMask.DataProcessor.ImportDto
{
    using Newtonsoft.Json;
    using System.ComponentModel.DataAnnotations;

    public class ImportEmployeesDto
    {
        [Required]
        [MinLength(3)]
        [MaxLength(40)]
        [RegularExpression(@"^[A-Za-z0-9]{3,}$")]
        [JsonProperty("Username")]
        public string Username { get; set; } = null!;

        [Required]
        [EmailAddress]
        [JsonProperty("Email")]
        public string Email { get; set; } = null!;

        [Required]
        [RegularExpression(@"^\d{3}\-\d{3}\-\d{4}$")]
        [JsonProperty("Phone")]
        public string Phone { get; set; } = null!;

        [JsonProperty("Tasks")]
        public int[] Tasks { get; set; } = null!;
    }
}
