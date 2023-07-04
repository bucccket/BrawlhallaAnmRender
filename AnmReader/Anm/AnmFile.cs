using BrawlhallaANMReader.ANM.Utils;
using System.Xml.Serialization;

namespace BrawlhallaANMReader.ANM.Anm
{
    ///<summary>Class <c>AnmFile</c> is used to read ANM files.</summary>
    ///<remarks>This is only compatible with animation files from patch 7.03 onwards (Battle Pass Season 7).</remarks>
    ///<see>https://www.brawlhalla.com/news/battle-pass-season-7-valhallaquest-patch-7-03/</see>
    [XmlRootAttribute("AnmFileXml", IsNullable = false)]
    public class AnmFile
    {
        ///<value>The name of the ANM file.</value>
        [XmlAttribute]
        public string Name { get; set; } = default!;

        ///<value>The file header.</value>
        public byte[] Header { get; set; } = default!;

        ///<value>The animation stores in the ANM file.</value>
        public List<AnmStore> Stores { get; set; } = new();

        ///<value>The <c>ByteStream</c> used to read the ANM file.</value>
        [field: NonSerializedAttribute()]
        protected ByteStream buffer = default!;

        ///<summary>Initialises a new ANM parser.</summary>
        public AnmFile() { }

        ///<summary>Parses an ANM file.</summary>
        ///<param name="path">The path to the ANM file.</param>
        ///<exception cref="FileNotFoundException">Thrown when the file does not exist.</exception>
        ///<exception cref="IOException">Thrown when the file cannot be read.</exception>		
        ///<exception cref="AnmParsingException">Thrown when the <c>ByteStream</c> is too short.</exception>
        public void Parse(string path)
        {
            try
            {
                byte[] file = File.ReadAllBytes(path);
                Name = Path.GetFileNameWithoutExtension(path);
                Header = new byte[4];
                Array.Copy(file, Header, 4);
                byte[] compressed_data = new byte[file.Length - 4];
                Array.Copy(file, 4, compressed_data, 0, compressed_data.Length);
                byte[] decompressed_data = Compression.InflateBuffer(compressed_data);
                buffer = new(decompressed_data);
                while (buffer.ReadBool())
                {
                    AnmStore store = new();
                    store.Parse(buffer);
                    Stores.Add(store);
                }
            }
            catch (Exception e)
            {
                Logger.Error($"AnmFile: {e.Message}");
                throw;
            }
        }

        ///<summary>Gets an animation store by name.</summary>
        ///<param name="index">The name of the animation store to get.</param>
        ///<returns>The animation store with the given name.</returns>
        ///<exception cref="AnmParsingException">Thrown when the animation store with the given name does not exist.</exception>
        public AnmStore GetStoreByName(string index)
        {
            foreach (AnmStore store in Stores) if (store.Name.Contains(index)) return store;
            Logger.Error($"AnmFile: Animation store with name {index} does not exist.");
            throw new AnmParsingException($"Animation store with name {index} does not exist.");
        }

        ///<summary>Serialises the ANM into an XML file.</summary>
        ///<param name="path">The path to save the XML file to.</param>
        public void ToXml(string path)
        {
            XmlSerializer serialiser = new(this.GetType());
            using StreamWriter writer = File.CreateText(path);
            serialiser.Serialize(writer, this);
        }
    }
}
