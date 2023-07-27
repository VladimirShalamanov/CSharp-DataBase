namespace Trucks.DataProcessor.ImportDto
{
    using System.Xml.Serialization;
    using System.ComponentModel.DataAnnotations;

    using SettingsProp;

    [XmlType("Despatcher")]
    public class ImportDespatcher
    {
        [Required]
        [MinLength(Constants.DespatcherNameMinLength)]
        [MaxLength(Constants.DespatcherNameMaxLength)]
        [XmlElement("Name")]
        public string Name { get; set; } = null!;

        [XmlElement("Position")]
        public string Position { get; set; }

        [XmlArray("Trucks")]
        public ImportDespatcherWithTruck[] Trucks { get; set; } = null!;
    }
}
