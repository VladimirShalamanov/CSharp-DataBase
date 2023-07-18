﻿namespace CarDealer.DTOs.Import;

using Newtonsoft.Json;

public class ImportCustomerDto
{
    //"name": "Emmitt Benally",
    //"birthDate": "1993-11-20T00:00:00",
    //"isYoungDriver": true

    [JsonProperty("name")]
    public string Name { get; set; } = null!;

    [JsonProperty("birthDate")]
    public DateTime BirthDate { get; set; }

    [JsonProperty("isYoungDriver")]
    public bool IsYoungDriver { get; set; }
}
