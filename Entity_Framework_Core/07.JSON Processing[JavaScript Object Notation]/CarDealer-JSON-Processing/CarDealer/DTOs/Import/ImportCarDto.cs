namespace CarDealer.DTOs.Import;

using Newtonsoft.Json;

public class ImportCarDto
{
    //"make": "Opel",
    //"model": "Astra",
    //"traveledDistance": 516628215,
    //"partsId": [
    //  39,
    //  62,
    //  72
    //]

    [JsonProperty("make")]
    public string Make { get; set; } = null!;

    [JsonProperty("model")]
    public string Model { get; set; } = null!;

    [JsonProperty("traveledDistance")]
    public long TraveledDistance { get; set; }

    [JsonProperty("partsId")]
    public int[] PartsId { get; set; } = null!;
}
