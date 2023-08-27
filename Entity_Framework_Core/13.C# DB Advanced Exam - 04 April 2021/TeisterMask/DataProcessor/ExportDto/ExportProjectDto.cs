namespace TeisterMask.DataProcessor.ExportDto
{
    using System.Xml.Serialization;

    [XmlType("Project")]
    public class ExportProjectDto
    {
        [XmlAttribute("TasksCount")]
        public int TasksCount { get; set; }

        [XmlElement("ProjectName")]
        public string ProjectName { get; set; } = null!;

        [XmlElement("HasEndDate")]
        public string HasEndDate { get; set; } = null!;

        [XmlArray("Tasks")]
        public ExportTaskDto[] Tasks { get; set; } = null!;
    }


    //[XmlType("Task")]
    //public class ExportTaskDto
    //{
    //    [XmlElement("Name")]
    //    public string Name { get; set; } = null!;

    //    [XmlElement("Label")]
    //    public string Label { get; set; } = null!;
    //}
}
