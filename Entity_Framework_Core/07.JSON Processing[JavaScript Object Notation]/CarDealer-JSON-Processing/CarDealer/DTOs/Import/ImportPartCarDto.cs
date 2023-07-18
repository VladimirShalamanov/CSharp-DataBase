namespace CarDealer.DTOs.Import;

using Newtonsoft.Json;

public class ImportPartCarDto
{
    [JsonProperty("partId")]
    public int Id { get; set; }
}
