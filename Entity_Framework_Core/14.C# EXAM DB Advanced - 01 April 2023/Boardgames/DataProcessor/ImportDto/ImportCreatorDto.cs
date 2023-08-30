namespace Boardgames.DataProcessor.ImportDto
{
    using System.ComponentModel.DataAnnotations;
    using System.Xml.Serialization;

    [XmlType("Creator")]
    public class ImportCreatorDto
    {
        [Required]
        [MinLength(2)]
        [MaxLength(7)]
        [XmlElement("FirstName")]
        public string FirstName { get; set; } = null!;

        [Required]
        [MinLength(2)]
        [MaxLength(7)]
        [XmlElement("LastName")]
        public string LastName { get; set; } = null!;

        [XmlArray("Boardgames")]
        public ImportBoardgamesDto[] Boardgames { get; set; } = null!;
    }

    [XmlType("Boardgame")]
    public class ImportBoardgamesDto
    {
        [Required]
        [MinLength(10)]
        [MaxLength(20)]
        [XmlElement("Name")]
        public string Name { get; set; } = null!;

        [Range(1, 10.00)]
        [XmlElement("Rating")]
        public double Rating { get; set; }

        [Range(2018, 2023)]
        [XmlElement("YearPublished")]
        public int YearPublished { get; set; }

        [Required]
        [Range(0, 4)]
        [XmlElement("CategoryType")]
        public int CategoryType { get; set; }

        [Required]
        [XmlElement("Mechanics")]
        public string Mechanics { get; set; } = null!;
    }
}
