using BrawlhallaANMReader.utils;
using System.Xml.Serialization;

namespace BrawlhallaANMReader.Anm
{
    ///<summary>Class <c>AnmFrame</c> represents a frame in an animation.</summary>
    public class AnmFrame
    {
        ///<value>The ID of the frame.</value>
        public ushort FrameID { get; set; } = default!;

        ///<value>The first offset of the frame.</value>
        public Point OffsetA { get; set; } = new();

        ///<value>The second offset of the frame.</value>
        public Point OffsetB { get; set; } = new();

        ///<value>The rotation of the frame.</value>
        ///<remarks>This value is seemingly unused and appears to always be NaN.</remarks>
        public double Rotation { get; set; } = default!;

        ///<value>Number of bones in the frame.</value>
        public short BonesCount { get; set; } = default!;

        ///<value>Bones in the frame.</value>
        public List<AnmBone> Bones { get; set; } = new();

        ///<summary>Initialises a new frame.</summary>
        public AnmFrame() { }

        ///<summary>Parses a frame from an animation.</summary>
        ///<param name="buffer">The animation to parse from.</param>
        ///<param name="last_frame">The previous frame.</param>
        ///<exception cref="AnmParsingException">Thrown when the <c>ByteStream</c> is too short.</exception>
        public void Parse(ByteStream buffer, AnmFrame? last_frame)
        {
            try
            {
                FrameID = buffer.ReadUShort();
                OffsetA.Parse(buffer);
                OffsetB.Parse(buffer);
                Rotation = buffer.ReadDouble();
                BonesCount = buffer.ReadShort();
            }
            catch (IndexOutOfRangeException)
            {
                Logger.Error("AnmFrame: Frame parsing error.  Buffer reached end unexpectedly.");
                throw new AnmParsingException("Frame parsing error.  Buffer reached end unexpectedly.");
            }
            for (int i = 0; i < BonesCount; i++)
            {
                AnmBone bone = new();
                if (buffer.ReadBool())
                {
                    bone = last_frame!.Bones[i].Clone();
                    if (!buffer.ReadBool()) bone.BoneMovieClipFrame = buffer.ReadShort();
                }
                else bone.Parse(buffer);
                Bones.Add(bone);
            }
        }
    }

    ///<summary>Class <c>Point</c> represents a point in 2D space.</summary>
    public class Point
    {
        ///<value>The X coordinate of the point.</value>
        [XmlAttribute]
        public double X { get; set; } = default!;

        ///<value>The Y coordinate of the point.</value>
        [XmlAttribute]
        public double Y { get; set; } = default!;

        ///<summary>Initialises a new point.</summary>
        public Point() { }

        ///<summary>Initialises a new point.</summary>
        ///<param name="x">The X coordinate of the point.</param>
        ///<param name="y">The Y coordinate of the point.</param>
        public Point(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        ///<summary>Parses a point from an animation.</summary>
        ///<param name="buffer">The animation to parse from.</param>
        ///<exception cref="AnmParsingException">Thrown when the <c>ByteStream</c> is too short.</exception>
        public void Parse(ByteStream buffer)
        {
            try
            {
                if (!buffer.ReadBool())
                {
                    X = 0;
                    Y = 0;
                    return;
                }
                X = buffer.ReadDouble();
                Y = buffer.ReadDouble();
            }
            catch (IndexOutOfRangeException)
            {
                Logger.Error("AnmFrame: Point parsing error.  Buffer reached end unexpectedly.");
                throw new AnmParsingException("Point parsing error.  Buffer reached end unexpectedly.");
            }
        }
    }
}
