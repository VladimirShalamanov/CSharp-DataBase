namespace Trucks.DataProcessor.ImportDto
{
    using System.Xml.Serialization;
    using System.ComponentModel.DataAnnotations;

    using SettingsProp;
    using Trucks.Data.Models.Enums;

    [XmlType("Truck")]
    public class ImportDespatcherWithTruck
    {
        [Required]
        [RegularExpression(Constants.TruckRegistrationNumberRegex)]
        [XmlElement("RegistrationNumber")]
        public string RegistrationNumber { get; set; } = null!;

        [Required]
        [StringLength(Constants.TruckVinNumberLength)]
        [XmlElement("VinNumber")]
        public string VinNumber { get; set; } = null!;

        [Range(950, 1420)]
        [XmlElement("TankCapacity")]
        public int TankCapacity { get; set; }

        [Range(5000, 29000)]
        [XmlElement("CargoCapacity")]
        public int CargoCapacity { get; set; }

        [Required]
        [Range(0 ,3)]
        [XmlElement("CategoryType")]
        public int CategoryType { get; set; }

        [Required]
        [Range(0, 4)]
        [XmlElement("MakeType")]
        public int MakeType { get; set; }
    }
}
