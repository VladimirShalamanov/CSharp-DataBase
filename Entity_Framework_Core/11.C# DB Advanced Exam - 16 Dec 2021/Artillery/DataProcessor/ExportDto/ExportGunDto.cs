namespace Artillery.DataProcessor.ExportDto
{
    using System.ComponentModel.DataAnnotations;
    using System.Xml.Serialization;

    [XmlType("Gun")]
    public class ExportGunDto
    {
        [Required]
        [XmlAttribute("Manufacturer")]
        public string ManufacturerName { get; set; } = null!;

        [Required]
        [XmlAttribute("GunType")]
        public string GunType { get; set; } = null!;

        [XmlAttribute("GunWeight")]
        public int GunWeight { get; set; }

        [XmlAttribute("BarrelLength")]
        public double BarrelLength { get; set; }

        [XmlAttribute("Range")]
        public int Range { get; set; }

        [XmlArray("Countries")]
        public ExportGunWithCountryDto[] Countries { get; set; } = null!;
    }

    [XmlType("Country")]
    public class ExportGunWithCountryDto
    {
        [Required]
        [XmlAttribute("Country")]
        public string CountryName { get; set; } = null!;

        [XmlAttribute("ArmySize")]
        public int ArmySize { get; set; }
    }
}