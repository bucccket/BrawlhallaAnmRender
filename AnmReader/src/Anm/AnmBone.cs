using BrawlhallaANMReader.utils;
using System.Xml.Serialization;

namespace BrawlhallaANMReader.Anm
{
	///<summary>Class <c>AnmBone</c> represents a bone.</summary>
	public class AnmBone
	{
		///<value>The ID of the bone.</value>
		[XmlAttribute]
		public ushort BoneID { get; set; } = default!;

		///<value>The offset of the bone.</value>
		public Vector2D Offset { get; set; } = new();

		///<value>The transformation matrix of the bone.</value>
		public TransformationMatrix TransformationMatrix { get; set; } = new();

		///<value>The frame of the bone.</value>
		[XmlAttribute]
		public short BoneMovieClipFrame { get; set; } = default!;

		///<value>The affine transformation matrix of the bone.</value>
		public AffineTransformationMatrix? AffineMatrix { get; set; }

		///<value>The opacity of the bone.</value>
		[XmlAttribute]
		public double Opacity { get; set; } = default!;

		///<value>The X scale of the bone.</value>
		[XmlAttribute]
		public float ScaleX { get => TransformationMatrix.ScaleX; }

		///<value>The Y scale of the bone.</value>
		[XmlAttribute]
		public float ScaleY { get => TransformationMatrix.ScaleY; }

		///<value>The first rotational skew of the bone.</value>
		[XmlAttribute]
		public float RotateSkew0 { get => TransformationMatrix.RotateSkew0; }

		///<value>The second rotational skew of the bone.</value>
		[XmlAttribute]
		public float RotateSkew1 { get => TransformationMatrix.RotateSkew1; }

		///<value>The X translation of the bone.</value>
		[XmlAttribute]
		public float TranslateX { get => Offset.X; }

		///<value>The Y translation of the bone.</value>
		[XmlAttribute]
		public float TranslateY { get => Offset.Y; }

		///<summary>Initialises a new bone.</summary>
		public AnmBone() { }

		///<summary>Clones a bone from another bone.</summary>
		///<returns>The cloned bone.</returns>
		public AnmBone Clone()
		{
			AnmBone bone = new()
			{
				BoneID = BoneID,
				Offset = new(TranslateX, TranslateY),
				TransformationMatrix = new(ScaleX, ScaleY, RotateSkew0, RotateSkew1),
				BoneMovieClipFrame = BoneMovieClipFrame,
				Opacity = Opacity
			};
			if (AffineMatrix != null) bone.AffineMatrix = new(AffineMatrix.A, AffineMatrix.B, AffineMatrix.C, AffineMatrix.D, AffineMatrix.Tx, AffineMatrix.Ty);
			else bone.AffineMatrix = null;
			return bone;
		}

		///<summary>Parses a bone from a frame.</summary>
		///<param name="buffer">The frame to parse from.</param>
		///<exception cref="AnmParsingException">Thrown when the <c>ByteStream</c> is too short.</exception>
		public void Parse(ByteStream buffer)
		{
			try
			{
				BoneID = buffer.ReadUShort();
				bool opaque = buffer.ReadBool();
				TransformationMatrix.Parse(buffer);
				Offset.Parse(buffer);
				BoneMovieClipFrame = buffer.ReadShort();
				if (!opaque) Opacity = buffer.ReadByte() / (byte)255;
				else Opacity = 1;
				if (buffer.ReadBool())
				{
					AffineMatrix = new();
					AffineMatrix.Parse(buffer);
				}
			}
			catch (IndexOutOfRangeException)
			{
				Logger.Error("AnmBone: Bone parsing error.  Buffer reached end unexpectedly.");
				throw new AnmParsingException("Bone parsing error.  Buffer reached end unexpectedly.");
			}
		}
	}

	///<summary>Class <c>Vector2D</c> represents a vector in 2D space.</summary>
	public class Vector2D
	{
		///<value>The X coordinate of the vector.</value>
		[XmlAttribute]
		public float X { get; set; } = default!;

		///<value>The Y coordinate of the vector.</value>
		[XmlAttribute]
		public float Y { get; set; } = default!;

		///<summary>Initialises a new vector.</summary>
		public Vector2D() { }

		///<summary>Initialises a new vector with the given coordinates.</summary>
		///<param name="x">The X coordinate of the vector.</param>
		///<param name="y">The Y coordinate of the vector.</param>
		public Vector2D(float x, float y)
		{
			X = x;
			Y = y;
		}

		///<summary>Parses a vector from a frame.</summary>
		///<param name="buffer">The frame to parse from.</param>
		///<exception cref="AnmParsingException">Thrown when the <c>ByteStream</c> is too short.</exception>
		public void Parse(ByteStream buffer)
		{
			try
			{
				X = buffer.ReadFloat();
				Y = buffer.ReadFloat();
			}
			catch (IndexOutOfRangeException)
			{
				Logger.Error("AnmBone: Vector parsing error.  Buffer reached end unexpectedly.");
				throw new AnmParsingException("Vector parsing error.  Buffer reached end unexpectedly.");
			}
		}
	}

	///<summary>Class <c>TransformationMatrix</c> represents a transformation matrix.</summary>
	public class TransformationMatrix
	{
		///<value>The X scale of the matrix.</value>
		[XmlAttribute]
		public float ScaleX { get; set; } = default!;

		///<value>The Y scale of the matrix.</value>
		[XmlAttribute]
		public float ScaleY { get; set; } = default!;

		///<value>The first rotational skew of the matrix.</value>
		[XmlAttribute]
		public float RotateSkew0 { get; set; } = default!;

		///<value>The second rotational skew of the matrix.</value>
		[XmlAttribute]
		public float RotateSkew1 { get; set; } = default!;

		///<summary>Initialises a new transformation matrix.</summary>
		public TransformationMatrix() { }

		///<summary>Initialises a new transformation matrix with the given values.</summary>
		///<param name="scale_x">The X scale of the matrix.</param>
		///<param name="scale_y">The Y scale of the matrix.</param>
		///<param name="rotate_skew_0">The first rotational skew of the matrix.</param>
		///<param name="rotate_skew_1">The second rotational skew of the matrix.</param>
		public TransformationMatrix(float scale_x, float scale_y, float rotate_skew_0, float rotate_skew_1)
		{
			ScaleX = scale_x;
			ScaleY = scale_y;
			RotateSkew0 = rotate_skew_0;
			RotateSkew1 = rotate_skew_1;
		}

		///<summary>Parses a transformation matrix from a frame.</summary>
		///<param name="buffer">The frame to parse from.</param>
		///<exception cref="AnmParsingException">Thrown when the <c>ByteStream</c> is too short.</exception>
		public void Parse(ByteStream buffer)
		{
			try
			{
				bool identity = false;
				bool symmetric = false;
				if (buffer.ReadBool())
				{
					if (buffer.ReadBool()) identity = true;
					else symmetric = true;
				}
				if (identity)
				{
					ScaleX = 1;
					ScaleY = 1;
					RotateSkew0 = 0;
					RotateSkew1 = 0;
				}
				else
				{
					ScaleX = buffer.ReadFloat();
					RotateSkew0 = buffer.ReadFloat();
					if (symmetric)
					{
						RotateSkew1 = RotateSkew0;
						ScaleY = -ScaleX;
					}
					else
					{
						RotateSkew1 = buffer.ReadFloat();
						ScaleY = buffer.ReadFloat();
					}
				}
			}
			catch (IndexOutOfRangeException)
			{
				Logger.Error("AnmBone: Transformation matrix parsing error.  Buffer reached end unexpectedly.");
				throw new AnmParsingException("Transformation matrix parsing error.  Buffer reached end unexpectedly.");
			}
		}
	}

	///<summary>Class <c>AffineTransformationMatrix</c> represents an affine transformation matrix.</summary>
	public class AffineTransformationMatrix
	{
		///<value>The A value of the matrix.</value>
		[XmlAttribute]
		public float A { get; set; } = default!;

		///<value>The B value of the matrix.</value>
		[XmlAttribute]
		public float B { get; set; } = default!;

		///<value>The C value of the matrix.</value>
		[XmlAttribute]
		public float C { get; set; } = default!;

		///<value>The D value of the matrix.</value>
		[XmlAttribute]
		public float D { get; set; } = default!;

		///<value>The Tx value of the matrix.</value>
		[XmlAttribute]
		public float Tx { get; set; } = default!;

		///<value>The Ty value of the matrix.</value>
		[XmlAttribute]
		public float Ty { get; set; } = default!;

		///<summary>Initialises a new affine transformation matrix.</summary>
		public AffineTransformationMatrix() { }

		///<summary>Initialises a new affine transformation matrix with the given values.</summary>
		///<param name="a">The A value of the matrix.</param>
		///<param name="b">The B value of the matrix.</param>
		///<param name="c">The C value of the matrix.</param>
		///<param name="d">The D value of the matrix.</param>
		///<param name="tx">The Tx value of the matrix.</param>
		///<param name="ty">The Ty value of the matrix.</param>
		public AffineTransformationMatrix(float a, float b, float c, float d, float tx, float ty)
		{
			A = a;
			B = b;
			C = c;
			D = d;
			Tx = tx;
			Ty = ty;
		}

		///<summary>Parses an affine transformation matrix from a bone.</summary>
		///<param name="buffer">The bone to parse from.</param>
		///<exception cref="AnmParsingException">Thrown when the <c>ByteStream</c> is too short.</exception>
		public void Parse(ByteStream buffer)
		{
			try
			{
				A = buffer.ReadFloat();
				B = buffer.ReadFloat();
				C = buffer.ReadFloat();
				D = buffer.ReadFloat();
				Tx = buffer.ReadFloat();
				Ty = buffer.ReadFloat();
			}
			catch (IndexOutOfRangeException)
			{
				Logger.Error("AnmBone: Affine transformation matrix parsing error.  Buffer reached end unexpectedly.");
				throw new AnmParsingException("Affine transformation matrix parsing error.  Buffer reached end unexpectedly.");
			}
		}
	}
}
