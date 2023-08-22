namespace Theatre.DataProcessor.ImportDto
{
    using System.ComponentModel.DataAnnotations.Schema;
    using System.ComponentModel.DataAnnotations;
    using Theatre.Data.Models;
    using System.Xml.Serialization;

    [XmlType("Cast")]
    public class ImportCastDto
    {
        [Required]
        [MinLength(4)]
        [MaxLength(30)]
        [XmlElement("FullName")]
        public string FullName { get; set; } = null!;

        [XmlElement("IsMainCharacter")]
        public bool IsMainCharacter { get; set; }

        [Required]
        [RegularExpression(@"^\+44-(\d{2})-(\d{3})-(\d{4})$")]
        [XmlElement("PhoneNumber")]
        public string PhoneNumber { get; set; } = null!;

        [XmlElement("PlayId")]
        public int PlayId { get; set; }
    }
}
