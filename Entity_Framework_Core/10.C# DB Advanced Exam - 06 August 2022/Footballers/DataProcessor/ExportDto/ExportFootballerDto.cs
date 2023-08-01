namespace Footballers.DataProcessor.ExportDto
{
    using System.ComponentModel.DataAnnotations;
    using System.Xml.Serialization;

    [XmlType("Footballer")]
    public class ExportFootballerDto
    {
        [Required]
        [XmlElement("Name")]
        public string Name { get; set; } = null!;

        [Required]
        [XmlElement("Position")]
        public string Position { get; set; } = null!;
    }
}
