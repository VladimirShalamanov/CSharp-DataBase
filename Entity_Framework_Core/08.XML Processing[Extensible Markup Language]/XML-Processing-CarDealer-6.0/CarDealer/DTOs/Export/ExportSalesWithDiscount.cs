namespace CarDealer.DTOs.Export;

using System.Runtime.CompilerServices;
using System.Xml.Serialization;

[XmlType("sale")]
public class ExportSalesWithDiscount
{
    [XmlElement("car")]
    public CarSaleDto Car { get; set; } = null!;

    [XmlElement("discount")]
    public decimal Discount { get; set; }

    [XmlElement("customer-name")]
    public string CustomerName { get; set; } = null!;

    [XmlElement("price")]
    public decimal Price { get; set; }

    [XmlElement("price-with-discount")]
    public decimal PriceWithDiscount { get; set; }
}

[XmlType("car")]
public class CarSaleDto
{
    [XmlAttribute("make")]
    public string Make { get; set; } = null!;
    [XmlAttribute("model")]
    public string Model { get; set; } = null!;
    [XmlAttribute("traveled-distance")]
    public long TraveledDistance { get; set; }
}
