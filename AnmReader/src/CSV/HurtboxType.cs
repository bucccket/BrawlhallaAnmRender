namespace BrawlhallaANMReader.CSV
{
    public class HurtboxType
    {
        [CsvIgnore]
        public static IList<HurtboxType>? HurtboxTypes { get; set; } = default;
        public string HurtboxName { get; set; } = default!;
        public int HurtboxID { get; set; } = default!;
        public string? AnimClass { get; set; }
        public string? AnimName { get; set; }
        public string Width { get; set; } = default!;
        public string Height { get; set; } = default!;
        [CsvIgnore]
        public List<string> OffsetXValues { get; } = new();
        public string? OffsetX { get => string.Join(",", OffsetXValues); set => OffsetXValues.AddRange(value.Split(',')); }
        [CsvIgnore]
        public List<string> OffsetYValues { get; } = new();
        public string? OffsetY { get => string.Join(",", OffsetYValues); set => OffsetYValues.AddRange(value.Split(',')); }
        public string? Frames { get; set; }
        public bool? IgnoreHeightValidation { get; set; }

        private readonly CsvSerializer<HurtboxType> _csv = new()
        {
            HasHeader = true
        };

        public HurtboxType()
        {

        }

        public void Parse(Stream stream)
        {
            HurtboxTypes = _csv.Deserialize(stream);
        }
        public void Write(FileStream stream)
        {
            _csv.Serialize(stream, HurtboxTypes ?? throw new NullReferenceException());
        }
    }
}
