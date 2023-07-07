using BrawlhallaANMReader.Swz.CSV;

namespace BrawlhallaANMReader.Swz.AbstractTypes;
public record SpriteData : IAbstractType
{
    public static List<SpriteData> ISpriteData { get; set; } = new();
    public string SetName { get; set; } = default!;
    public string BoneName { get; set; } = default!;
    public string File { get; set; } = default!;
    public string Width { get; set; } = default!;
    public string Height { get; set; } = default!;
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
    public string xOffset { get; set; } = default!;
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
    public string yOffset { get; set; } = default!;

    private static readonly CsvSerializer<SpriteData> _csv = new()
    {
        HasHeader = true
    };

    public SpriteData()
    {

    }

    public static void Parse(Stream stream)
    {
        ISpriteData.AddRange(_csv.Deserialize(stream));
    }
    public static void Write(FileStream stream)
    {
        _csv.Serialize(stream, ISpriteData ?? throw new NullReferenceException());
    }
}
