namespace Theatre.DataProcessor.ExportDto
{
    using System.ComponentModel.DataAnnotations;
    using System.Xml.Serialization;

    [XmlType("Play")]
    public class ExportPlaysDto
    {
        [Required]
        [XmlAttribute("Title")]
        public string Title { get; set; } = null!;

        [Required]
        [XmlAttribute("Duration")]
        public string Duration { get; set; } = null!;

        [XmlAttribute("Rating")]
        public string Rating { get; set; }

        [Required]
        [XmlAttribute("Genre")]
        public string Genre { get; set; } = null!;

        [XmlArray("Actors")]
        public ExportActorsDto[] Actors { get; set; } = null!;
    }

    [XmlType("Actor")]
    public class ExportActorsDto
    {
        [XmlAttribute("FullName")]
        public string FullName { get; set; } = null!;

        [XmlAttribute("MainCharacter")]
        public string MainCharacter { get; set; } = null!;
    }
}
