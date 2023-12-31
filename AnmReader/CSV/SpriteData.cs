﻿namespace BrawlhallaANMReader.ANM.CSV
{
    public class SpriteData
    {
        public static IList<SpriteData>? ISpriteData { get; set; } = default;
        public string SetName { get; set; } = default!;
        public string BoneName { get; set; } = default!;
        public string File { get; set; } = default!;
        public string Width { get; set; } = default!;
        public string Height { get; set; } = default!;
        public string xOffset { get; set; } = default!;
        public string yOffset { get; set; } = default!;

        private readonly CsvSerializer<SpriteData> _csv = new()
        {
            HasHeader = true
        };

        public SpriteData()
        {

        }

        public void Parse(Stream stream)
        {
            ISpriteData = _csv.Deserialize(stream);
        }
        public void Write(FileStream stream)
        {
            _csv.Serialize(stream, ISpriteData ?? throw new NullReferenceException());
        }
    }
}
