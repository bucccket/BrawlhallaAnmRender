using System.Xml;
using System.Xml.Serialization;
using BrawlhallaANMReader.utils;

namespace BrawlhallaANMReader.AbstractTypes
{
	///<summary>Class <c>Deserialiser</c> is used to deserialise files that contain lists of AbstractTypes.</summary>
	internal static class Deserialiser
	{
		///<summary>Deserialises an XML file.</summary>
		///<typeparam name="type">The type of the object to deserialise.</typeparam>
		///<param name="xml">The XML file to deserialise.</param>
		///<param name="root">The root element of the XML file.</param>
		///<param name="obj">The name of the object to deserialise.</param>
		///<param name="template_match">The string used to identify the template object.  This object will be skipped.</param>
		///<param name="policy">The policy to use when the XML file contains invalid objects.</param>
		///<returns>A list of objects of type <c>type</c>.</returns>
		///<exception cref="NullReferenceException">Thrown when no objects of type <c>type</c> are found.</exception>
		///<exception cref="InvalidOperationException">Thrown when the XML file is invalid.</exception>
		internal static List<type> XmlListDeserialise<type>(string xml, string root, string obj, string? template_match = null, DataAcceptancePolicy policy = DataAcceptancePolicy.DeclineWholeFileOnInvalid) where type : class
		{
			XmlDocument doc = new();
			doc.LoadXml(xml);
			XmlNodeList? nodes = doc.DocumentElement?.SelectNodes($"/{root}/*");
			if (nodes == null || nodes.Count == 0)
			{
				Logger.Error($"Deserialiser: No objects of type {obj} found.");
				throw new NullReferenceException($"No objects of type {obj} found.");
			}
			XmlSerializer serialiser = new(typeof(type), new XmlRootAttribute("LanguageType"));
			List<type> list = new();
			foreach (XmlNode node in nodes)
			{
				try
				{
					if (template_match != null && node.InnerXml.Contains(template_match)) continue;
					using StringReader reader = new(node.OuterXml);
					type? element = (type?)serialiser.Deserialize(reader);
					if (element != null) list.Add(element);
					else
					{
						if (policy == DataAcceptancePolicy.DeclineWholeFileOnInvalid)
						{
							Logger.Error($"Deserialiser: Object of type other than {obj} found.");
							throw new InvalidOperationException($"Object of type other than {obj} found.");
						}
						else
						Logger.Warn($"Deserialiser: Invalid object of type {obj} found.  Skipping.");
					}
				}
				catch (InvalidOperationException)
				{
					if (policy == DataAcceptancePolicy.DeclineWholeFileOnInvalid)
					{
						Logger.Error("Deserialiser: The file contains invalid data.");
						throw;
					}
					else
					Logger.Warn("Deserialiser: Invalid data found.  Skipping.");
				}
				catch (Exception e)
				{
					Logger.Error($"Deserialiser: {e.Message}");
					throw;
				}
			}
			return list;
		}
	}

	///<summary>Enum <c>DataAcceptancePolicy</c> is used to specify how to handle invalid data.</summary>
	public enum DataAcceptancePolicy
	{
		///<summery><c>AlwaysParseValid</c> is used to parse the file even if it contains invalid data.</summery>
		AlwaysParseValid,
		///<summery><c>DeclineWholeFileOnInvalid</c> is used to decline parsing the file if it contains invalid data.</summery>
		DeclineWholeFileOnInvalid,
	}
}
