namespace Boardgames.DataProcessor.ExportDto
{
    using System.ComponentModel.DataAnnotations;
    using System.Xml.Serialization;

    [XmlType("Creator")]
    public class ExportCreatorDto
    {
        [XmlAttribute("BoardgamesCount")]
        public int BoardgamesCount { get; set; }

        [Required]
        [XmlElement("CreatorName")]
        public string CreatorName { get; set; } = null!;

        [XmlArray("Boardgames")]
        public ExportBoardgamesDto[] Boardgames { get; set; } = null!;
    }

    [XmlType("Boardgame")]
    public class ExportBoardgamesDto
    {
        [Required]
        [XmlElement("BoardgameName")]
        public string BoardgameName { get; set; } = null!;

        [XmlElement("BoardgameYearPublished")]
        public int BoardgameYearPublished { get; set; }
    }
}
