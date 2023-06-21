using System.Xml;
using System.Xml.Serialization;
using BrawlhallaANMReader.utils;

namespace BrawlhallaANMReader.AbstractTypes
{
	///<summary>Class <c>XmlListDeserialiser</c> is used to deserialise XML files that contain lists of AbstractTypes.</summary>
	internal static class XmlListDeserialiser
	{
		///<summary>Deserialises an XML file.</summary>
		///<typeparam name="type">The type of the object to deserialise.</typeparam>
		///<param name="xml">The XML file to deserialise.</param>
		///<param name="root">The root element of the XML file.</param>
		///<param name="obj">The name of the object to deserialise.</param>
		///<param name="template_match">The string used to identify the template object.  This object will be skipped.</param>
		internal static List<type> Deserialise<type>(string xml, string root, string obj, string? template_match = null) where type : class
		{
			XmlDocument doc = new();
			doc.LoadXml(xml);
			////TODO: null check
			XmlNodeList nodes = doc.DocumentElement.SelectNodes($"/{root}/*");
			XmlSerializer serialiser = new(typeof(type), new XmlRootAttribute("LanguageType"));
			List<type> list = new();
			foreach (XmlNode node in nodes) ////TODO: null check
			{
				try
				{
					if (template_match != null && node.InnerXml.Contains(template_match)) continue;
					using StringReader reader = new(node.OuterXml);
					list.Add((type)serialiser.Deserialize(reader)); ////TODO: null check
				}
				catch (InvalidOperationException)
				{
					Logger.Error("Invalid Object!");
					throw;
				}
				catch (Exception e)
				{
					Logger.Error(e.Message);
					throw;
				}
			}
			return list;
		}
	}
}
