namespace ProductShop.DTOs.Import;

using Newtonsoft.Json;

public class ImportCategoryGto
{
    [JsonProperty("name")]
    public string? Name { get; set; }
}
