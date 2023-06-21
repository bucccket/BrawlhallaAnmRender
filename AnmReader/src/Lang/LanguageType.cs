using System.Xml.Serialization;
using BrawlhallaANMReader.utils;
using BrawlhallaANMReader.AbstractTypes;

namespace BrawlhallaANMReader.Lang
{
	///<summery>Class <c>LanguageType</c> models a language and its properties.</summery>
	public record LanguageType
	{
		///<value>The singleton list that contains all languages.</value>
		[XmlElement("LanguageTypes")]
		public static List<LanguageType> LanguageTypes { get; } = new();

		///<value>Internal unique name of the language.</value>
		[XmlAttribute]
		public string LanguageName { get; set; } = "Template";

		///<value>Internal unique ID of the language.</value>
		public uint LanguageID { get; set; } = 0;

		///<value>Display name of the language.</value>
		public string DisplayName { get; set; } = "Template Language";

		///<value>Ubisoft DNA code of the language.</value>
		///<see>https://mdc-web-tomcat17.ubisoft.org/confluence/pages/viewpage.action?pageId=384996702#EventsGuidelinesforConsolesandPC-game.localization</see>
		public string DnaCode { get; set; } = "en-US";

		///<value>ISO 639-2 code of the language.</value>
		///<see>https://www.loc.gov/standards/iso639-2/php/code_list.php</see>
		public string IsoCode { get; set; } = "eng";

		///<value>Names of the linkage for the fonts.  Order must match with FontFiles column.  Empty if default (Eras).  Comma separated.</value>
		[XmlIgnore]
		public List<string> FontLinkageNames { get; } = new();

		///<value>Names of the linkage for the fonts.  Order must match with FontFiles column.  Empty if default (Eras).  Comma separated.</value>
		[XmlAttribute("FontLinkageNames")]
		public string FontLinkageNamesString { get => string.Join(",", FontLinkageNames); set => FontLinkageNames.AddRange(value.Split(',')); }

		///<value>Files that contain the font linkages, must match with FontLinkageNames.  Must be a valid file in bin/fontData.  Empty if default (Eras).  Comma separated.</value>
		[XmlIgnore]
		public List<string> FontFiles { get; } = new();

		///<value>Files that contain the font linkages, must match with FontLinkageNames.  Must be a valid file in bin/fontData.  Empty if default (Eras).  Comma separated.</value>
		[XmlAttribute("FontFiles")]
		public string FontFilesString { get => string.Join(",", FontFiles); set => FontFiles.AddRange(value.Split(',')); }

		///<value>True if this language is available, false if not.</value>
		[XmlIgnore]
		public bool Enabled { get; set; } = true;

		///<value>True if this language is available, false if not.</value>
		[XmlAttribute("Enabled")]
		public string EnabledString { get => Enabled ? "TRUE" : "FALSE"; set => Enabled = value.ToUpper() == "TRUE"; }

		///<value>True if this language has spaces between words, false if not.</value>
		[XmlIgnore]
		public bool HasSpaces { get; set; } = true;

		///<value>True if this language has spaces between words, false if not.</value>
		[XmlAttribute("HasSpaces")]
		public string HasSpacesString { get => HasSpaces ? "TRUE" : "FALSE"; set => HasSpaces = value.ToUpper() == "TRUE"; }

		///<summary>Initializes a new Language.</summary>
		public LanguageType() { }

		///<summary>Loading a language XML file.</summary>
		///<param name="xml">The XML string to parse.</param>
		public static void Parse(string xml)
		{
			try
			{
				LanguageTypes.Clear();
				LanguageTypes.AddRange(XmlListDeserialiser.Deserialise<LanguageType>(xml, "LanguageTypes", "LanguageType", "<LanguageID>0</LanguageID>"));
			}
			catch (InvalidOperationException)
			{
				Logger.Error("Invalid XML file; cleared list.");
				LanguageTypes.Clear();
				throw;
			}
			catch (Exception e)
			{
				Logger.Error(e.Message);
				throw;
			}
		}

		///<summary>Gets a language by its ID.</summary>
		///<param name="id">The ID of the language.</param>
		///<returns>The language with the given ID.</returns>
		public static LanguageType GetLanguageByID(uint id) => LanguageTypes.Find(language => language.LanguageID == id);
		////TODO: null check
	}
}
