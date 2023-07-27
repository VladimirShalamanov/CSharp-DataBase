namespace Trucks.DataProcessor.ExportDto
{
    using System.Xml.Serialization;
    using Trucks.Data.Models;

    [XmlType("Despatcher")]
    public class ExportDespatcher
    {
        [XmlElement("DespatcherName")]
        public string Name { get; set; } = null!;

        [XmlAttribute("TrucksCount")]
        public int TrucksCount { get; set; }

        [XmlArray("Trucks")]
        public ExportDespatcherWithTruck[] Trucks { get; set; } = null!;
    }
}
