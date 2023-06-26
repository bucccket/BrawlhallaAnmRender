using BrawlhallaANMReader.Lang;

namespace BrawlhallaANMReader.CSV
{
    public class HurtboxType
    {
        [CsvIgnore]
        public static List<HurtboxType> HurtboxTypes { get; } = new();
        public string HurtboxName { get; set; } = default!;
        public int HurtboxID { get; set; } = default!;
        public string? AnimClass { get; set; }
        public string? AnimName { get; set; }
        public string Width { get; set; } = default!;
        public string Height { get; set; } = default!;
        [CsvIgnore]
        public List<string> OffsetXValues { get; } = new();
        public string? OffsetX { get=> string.Join(",", OffsetXValues); set=>OffsetXValues.AddRange(value.Split(',')); }
        [CsvIgnore]
        public List<string> OffsetYValues { get; } = new();
        public string? OffsetY { get => string.Join(",", OffsetYValues); set => OffsetYValues.AddRange(value.Split(',')); }
        public string? Frames { get; set; }
        public bool? IgnoreHeightValidation { get; set; }
    }
}
