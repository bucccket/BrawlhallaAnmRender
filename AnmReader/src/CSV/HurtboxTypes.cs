namespace BrawlhallaANMReader.CSV
{
    public class HurtboxTypes
    {
        public string HurtboxName { get; set; } = "DEFAULT";
        public int HurtboxID { get; set; } = default!;
        public string? AnimClass { get; set; } = default;
        public string? AnimName { get; set; } = default;
        public string Width { get; set; } = default!;
        public string Height { get; set; } = default!;
        public string? OffsetX { get; set; } = default;
        public string? OffsetY { get; set; } = default;
        public string? Frames { get; set; } = default;
        public bool? IgnoreHeightValidation { get; set; }
    }
}
