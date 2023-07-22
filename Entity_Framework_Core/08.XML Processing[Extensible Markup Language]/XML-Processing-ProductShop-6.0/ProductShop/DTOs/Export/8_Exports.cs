namespace ProductShop.DTOs.Export;

using System.Xml.Serialization;

public class _8_Exports
{
    [XmlType("Users")]
    public class ExportUserCount
    {
        [XmlElement("count")]
        public int Count { get; set; }

        [XmlArray("users")]
        public UserInfo[] Users { get; set; } = null!;
    }

    [XmlType("User")]
    public class UserInfo
    {
        [XmlElement("firstName")]
        public string FirstName { get; set; } = null!;

        [XmlElement("lastName")]
        public string LastName { get; set; } = null!;

        [XmlElement("age")]
        public int? Age { get; set; }

        public SoldProductsCount SouldProducts { get; set; } = null!;
    }

    [XmlType("SoldProducts")]
    public class SoldProductsCount
    {
        [XmlElement("count")]
        public int Count { get; set; }

        [XmlArray("products")]
        public SoldProduct[] Products { get; set; } = null!;
    }

    [XmlType("Product")]
    public class SoldProduct
    {
        [XmlElement("name")]
        public string Name { get; set; } = null!;

        [XmlElement("price")]
        public decimal Price { get; set; }
    }
}
