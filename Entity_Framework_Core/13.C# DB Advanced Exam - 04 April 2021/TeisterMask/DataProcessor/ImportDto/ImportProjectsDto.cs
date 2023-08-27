namespace TeisterMask.DataProcessor.ImportDto
{
    using System.ComponentModel.DataAnnotations;
    using System.Xml.Serialization;

    [XmlType("Project")]
    public class ImportProjectsDto
    {
        [Required]
        [MinLength(2)]
        [MaxLength(40)]
        [XmlElement("Name")]
        public string Name { get; set; } = null!;

        [Required]
        [XmlElement("OpenDate")]
        public string OpenDate { get; set; } = null!;

        [XmlElement("DueDate")]
        public string? DueDate { get; set; }

        [XmlArray("Tasks")]
        public ImportTaskDto[] Tasks { get; set; } = null!;
    }

    [XmlType("Task")]
    public class ImportTaskDto
    {
        [Required]
        [MinLength(2)]
        [MaxLength(40)]
        [XmlElement("Name")]
        public string Name { get; set; } = null!;

        [Required]
        [XmlElement("OpenDate")]
        public string OpenDate { get; set; } = null!;

        [Required]
        [XmlElement("DueDate")]
        public string DueDate { get; set; } = null!;

        [Range(0, 3)]
        [XmlElement("ExecutionType")]
        public int ExecutionType { get; set; }

        [Range(0, 4)]
        [XmlElement("LabelType")]
        public int LabelType { get; set; }
    }
}
