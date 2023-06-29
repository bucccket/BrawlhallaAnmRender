using BrawlhallaANMReader.utils;
using System.Xml.Serialization;

namespace BrawlhallaANMReader.Anm
{
    ///<summary>Class <c>AnmAnimation</c> represents an animation.</summary>
    [Serializable()]
    public class AnmAnimation
    {
        ///<value>Name of the animation.</value>
        [XmlAttribute]
        public string Name { get; set; } = default!;

        ///<value>Number of frames in the animation.</value>
        public uint FrameCount { get; set; } = default!;

        ///<value>Index of the loop start frame.</value>
        protected uint m_loop_start { get; set; } = default!;

        ///<value>The loop start frame.</value>
        public AnmFrame LoopStart { get => Frames[(int)m_loop_start]; }

        ///<value>Index of the recovery start frame.</value>
        protected uint m_recovery_start { get; set; } = default!;

        ///<value>The recovery start frame.</value>
        public AnmFrame RecoveryStart { get => Frames[(int)m_recovery_start]; }

        ///<value>Index of the free start frame.</value>
        protected uint m_free_start { get; set; } = default!;

        ///<value>The free start frame.</value>
        public AnmFrame FreeStart { get => Frames[(int)m_free_start]; }

        ///<value>Index of the preview frame.</value>
        protected uint m_preview_frame { get; set; } = default!;

        ///<value>The preview frame.</value>
        public AnmFrame PreviewFrame { get => Frames[(int)m_preview_frame]; }

        ///<value>Index of the base start frame.</value>
        protected uint m_base_start { get; set; } = default!;

        ///<value>The base start frame.</value>
        public AnmFrame BaseStart { get => Frames[(int)m_base_start]; }

        ///<value>Number of animation data.</value>
        [field: NonSerializedAttribute()]
        public uint AnimDataSize { get; set; } = default!;

        ///<value>Animation data.</value>
        [field: NonSerializedAttribute()]
        public List<uint> AnimData { get; set; } = new();

        ///<value>Number of frame data.</value>
        [field: NonSerializedAttribute()]
        public uint FrameDataSize { get; set; } = default!;

        ///<value>Frames in the animation.</value>
        public List<AnmFrame> Frames { get; set; } = new();

        ///<summary>Initialises a new animation.</summary>
        public AnmAnimation() { }

        ///<summary>Parses an animation from an animation store.</summary>
        ///<param name="buffer">The animation store to parse from.</param>
        ///<exception cref="AnmParsingException">Thrown when the <c>ByteStream</c> is too short.</exception>
        public void Parse(ByteStream buffer)
        {
            try
            {
                Name = buffer.ReadString();
                FrameCount = buffer.ReadUInt();
                m_loop_start = buffer.ReadUInt();
                m_recovery_start = buffer.ReadUInt();
                m_free_start = buffer.ReadUInt();
                m_preview_frame = buffer.ReadUInt();
                m_base_start = buffer.ReadUInt();
                AnimDataSize = buffer.ReadUInt();
                for (int i = 0; i < AnimDataSize; i++) AnimData.Add(buffer.ReadUInt());
                FrameDataSize = buffer.ReadUInt();
            }
            catch (IndexOutOfRangeException)
            {
                Logger.Error("AnmAnimation: Animation store parsing error.  Buffer reached end unexpectedly.");
                throw new AnmParsingException();
            }
            for (int i = 0; i < FrameCount; i++)
            {
                AnmFrame frame = new();
                frame.Parse(buffer, (i != 0 ? Frames[i - 1] : null));
                Frames.Add(frame);
            }
        }
    }
}
