namespace Footballers.DataProcessor.ImportDto
{
    using System.ComponentModel.DataAnnotations;
    using System.Xml.Serialization;

    [XmlType("Footballer")]
    public class ImportFootballerDto
    {
        [Required]
        [MinLength(2)]
        [MaxLength(40)]
        [XmlElement("Name")]
        public string Name { get; set; } = null!;

        [Required]
        [XmlElement("ContractStartDate")]
        public string ContractStartDate { get; set; } = null!;

        [Required]
        [XmlElement("ContractEndDate")]
        public string ContractEndDate { get; set; } = null!;

        [Required]
        [Range(0, 4)]
        [XmlElement("BestSkillType")]
        public int BestSkillType { get; set; }

        [Required]
        [Range(0, 3)]
        [XmlElement("PositionType")]
        public int PositionType { get; set; }
    }
}
