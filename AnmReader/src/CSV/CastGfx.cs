using System.Runtime.Serialization;

namespace BrawlhallaANMReader.CSV
{
    [Serializable()]
    public class CastGfx : ISerializable
    {
        public string? AnimFile { get; set; } = default;
        public string? AnimClass { get; set; } = default;
        public string? AnimScale { get; set; } = default;
        public string? FireAndForget { get; set; } = default;
        public string? MoveAnimSpeed { get; set; } = default;
        public string? FlipAnim { get; set; } = default;
        public string? Tint { get; set; } = default;

        public CastGfx()
        {
        }
        public CastGfx(SerializationInfo info, StreamingContext ctxt)
        {
            AnimFile = info.GetValue("AnimFile", typeof(string)) as string;
            AnimClass = info.GetValue("AnimClass", typeof(string)) as string;
            AnimScale = info.GetValue("AnimScale", typeof(string)) as string;
            FireAndForget = info.GetValue("FireAndForget", typeof(string)) as string;
            MoveAnimSpeed = info.GetValue("MoveAnimSpeed", typeof(string)) as string;
            FlipAnim = info.GetValue("FlipAnim", typeof(string)) as string;
            Tint = info.GetValue("Tint", typeof(string)) as string;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("AnimFile", AnimFile);
            info.AddValue("AnimClass", AnimClass);
            info.AddValue("AnimScale", AnimScale);
            info.AddValue("FireAndForget", FireAndForget);
            info.AddValue("MoveAnimSpeed", MoveAnimSpeed);
            info.AddValue("FlipAnim", FlipAnim);
            info.AddValue("Tint", Tint);
        }
    }
}
