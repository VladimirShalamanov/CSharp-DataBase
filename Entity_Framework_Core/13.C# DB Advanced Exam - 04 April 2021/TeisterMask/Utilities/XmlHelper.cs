namespace TeisterMask.Utilities;

using System.Text;
using System.Xml.Serialization;

public class XmlHelper
{
    // import
    public T Deserialize<T>(string inputXml, string rootName)
    {
        XmlRootAttribute xmlRoot = new XmlRootAttribute(rootName);
        XmlSerializer xmlSerializer = new XmlSerializer(typeof(T), xmlRoot);

        using StringReader reader = new StringReader(inputXml);
        T extractDtos = (T)xmlSerializer.Deserialize(reader)!;

        return extractDtos;
    }

    // export
    public string Serialize<T>(T obj, string rootName)
    {
        var sb = new StringBuilder();

        XmlRootAttribute xmlRoot = new XmlRootAttribute(rootName);
        XmlSerializer xmlSerializer = new XmlSerializer(typeof(T), xmlRoot);

        XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
        namespaces.Add(string.Empty, string.Empty);

        using StringWriter writer = new StringWriter(sb);
        xmlSerializer.Serialize(writer, obj, namespaces);

        return sb.ToString().TrimEnd();
    }
}
