using System.IO.Compression;
using System.Text;

namespace BrawlhallaANMReader
{
	///<summary>Class <c>Utilities</c> is used to store utility functions.</summary>
	internal static class Utilities
	{
		///<summery>Decompresses a buffer using the ZLib compression algorithm.</summery>
		///<param name="compressed_buffer">The compressed stream to decompress.</param>
		///<returns>The decompressed buffer.</returns>
		internal static byte[] InflateBuffer(byte[] compressed_buffer)
		{
			using MemoryStream compressed_stream = new(compressed_buffer);
			ZLibStream decompressor = new(compressed_stream, CompressionMode.Decompress);
			byte[] array = new byte[1024];
			using MemoryStream decompressed_stream = new();
			using (decompressor)
			{
				int count;
				while ((count = decompressor.Read(array, 0, array.Length)) != 0) decompressed_stream.Write(array, 0, count);
			}
			return decompressed_stream.ToArray();
		}

		///<summery>Compresses a buffer using the ZLib compression algorithm.</summery>
		///<param name="uncompressed_buffer">The uncompressed stream to compress.</param>
		///<returns>The compressed buffer.</returns>
		internal static byte[] DeflateBuffer(byte[] uncompressed_buffer)
		{
			using MemoryStream compressed_stream = new();
			ZLibStream compressor = new(compressed_stream, CompressionLevel.SmallestSize);
			using (compressor) compressor.Write(uncompressed_buffer, 0, uncompressed_buffer.Length);
			return compressed_stream.ToArray();
		}

		internal static DateTime UnixTimeStampToDateTime(uint unix_time_stamp)
		{
			DateTime date_time = new(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
			date_time = date_time.AddSeconds(unix_time_stamp).ToLocalTime();
			return date_time;
		}
	}

	///<summary>Class <c>BitStream</c> is used to read bits from a byte array.</summary>
	internal class BitStream
	{
		///<value>The buffer to read from.</value>
		private readonly byte[] m_buffer;

		///<value>The current position in the buffer.</value>
		private int m_position;

		///<value>The current bit position in the buffer.</value>
		private byte m_bit_position;

		///<value>The masks used to read bits.</value>
		private static readonly int[] s_masks = new int[33] { 0, 1, 3, 7, 15, 31, 63, 127, 255, 511, 1023, 2047, 4095, 8191, 16383, 32767, 65535, 131071, 262143, 524287, 1048575, 2097151, 4194303, 8388607, 16777215, 33554431, 67108863, 134217727, 268435455, 536870911, 1073741823, 2147483647, -1 };

		///<summary>Initialises a new bit stream.</summary>
		///<param name="buffer">The buffer to read from.</param>
		internal BitStream(byte[] buffer)
		{
			m_buffer = buffer;
			m_position = 0;
			m_bit_position = 0;
		}

		///<value>The number of bits remaining in the buffer.</value>
		internal int RemainingBytes => m_buffer.Length - m_position;

		///<summary>Reads a specified number of bits from the buffer.</summary>
		///<param name="count">The number of bits to read.</param>
		///<returns>The bits read from the buffer.</returns>
		///<exception cref="System.IndexOutOfRangeException">Thrown when the number of bits to read is greater than the number of bits remaining in the buffer.</exception>
		internal int ReadBits(int count)
		{

			int result = 0;
			while (count != 0)
			{
				if (m_position >= m_buffer.Length * 8) throw new System.IndexOutOfRangeException("End of stream reached");
				bool bit = (m_buffer[m_position] & (1 << (7 - m_bit_position))) != 0;
				result |= (bit ? 1 : 0) << (count - 1);
				count--;
				m_bit_position++;
				if (m_bit_position == 8)
				{
					m_position++;
					m_bit_position = 0;
				}
			}
			return result;
		}

		///<summary>Reads a single bit from the buffer.</summary>
		///<returns>The bit read from the buffer.</returns>
		///<exception cref="System.IndexOutOfRangeException">Thrown when the number of bits to read is greater than the number of bits remaining in the buffer.</exception>
		internal bool ReadBool() => ReadBits(1) != 0;

		///<summary>Reads an 8-bit wide boolean from the buffer.</summary>
		///<returns>The boolean read from the buffer.</returns>
		///<exception cref="System.IndexOutOfRangeException">Thrown when the number of bits to read is greater than the number of bits remaining in the buffer.</exception>
		internal bool ReadBoolean() => ReadByte() != 0;

		///<summary>Reads a single byte from the buffer.</summary>
		///<returns>The byte read from the buffer.</returns>
		///<exception cref="System.IndexOutOfRangeException">Thrown when the number of bits to read is greater than the number of bits remaining in the buffer.</exception>
		internal byte ReadByte() => (byte)ReadBits(8);

		///<summary>Reads a specified number of bytes from the buffer.</summary>
		///<param name="count">The number of bytes to read.</param>
		///<returns>The bytes read from the buffer.</returns>
		///<exception cref="System.IndexOutOfRangeException">Thrown when the number of bytes to read is greater than the number of bytes remaining in the buffer.</exception>
		internal byte[] ReadBytes(uint count)
		{
			byte[] result = new byte[count];
			for (int i = 0; i < count; i++) result[i] = ReadByte();
			return result;
		}

		///<summary>Reads a single short from the buffer.</summary>
		///<returns>The short read from the buffer.</returns>
		///<exception cref="System.IndexOutOfRangeException">Thrown when the number of bits to read is greater than the number of bits remaining in the buffer.</exception>
		internal short ReadShort() => (short)ReadBits(16);

		///<summary>Reads a single int from the buffer.</summary>
		///<returns>The int read from the buffer.</returns>
		///<exception cref="System.IndexOutOfRangeException">Thrown when the number of bits to read is greater than the number of bits remaining in the buffer.</exception>
		internal int ReadInt() => ReadBits(32);

		///<summary>Reads a single long from the buffer.</summary>
		///<returns>The long read from the buffer.</returns>
		///<exception cref="System.IndexOutOfRangeException">Thrown when the number of bits to read is greater than the number of bits remaining in the buffer.</exception>
		internal long ReadLong()
		{
			byte[] bytes = ReadBytes(8);
			return BitConverter.ToInt64(bytes, 0);
		}

		///<summary>Reads a single char from the buffer.</summary>
		///<returns>The char read from the buffer.</returns>
		///<exception cref="System.IndexOutOfRangeException">Thrown when the number of bits to read is greater than the number of bits remaining in the buffer.</exception>
		internal char ReadChar() => (char)ReadBits(8);

		///<summary>Reads a string from the buffer.</summary>
		///<returns>The string read from the buffer.</returns>
		///<exception cref="System.IndexOutOfRangeException">Thrown when the number of bits to read is greater than the number of bits remaining in the buffer.</exception>
		///<exception cref="System.Text.DecoderFallbackException">Thrown when the string contains invalid characters.</exception>
		///<remarks>Strings are encoded as a short specifying the length of the string, followed by the string itself.</remarks>
		internal string ReadString() => Encoding.UTF8.GetString(ReadBytes((ushort)ReadShort()));

		///<summary>Reads a single float from the buffer.</summary>
		///<returns>The float read from the buffer.</returns>
		///<exception cref="System.IndexOutOfRangeException">Thrown when the number of bits to read is greater than the number of bits remaining in the buffer.</exception>
		internal float ReadFloat() => BitConverter.ToSingle(BitConverter.GetBytes(ReadInt()), 0);

		///<summary>Reads a double float from the buffer.</summary>
		///<returns>The double float read from the buffer.</returns>
		///<exception cref="System.IndexOutOfRangeException">Thrown when the number of bits to read is greater than the number of bits remaining in the buffer.</exception>
		internal double ReadDouble() => BitConverter.Int64BitsToDouble(ReadLong());
	}

	///<summary>Class <c>ByteStream</c> is used to read bytes from a byte array.</summary>
	public class ByteStream
	{
		///<value>The byte array to read from.</value>
		protected readonly byte[] m_buffer;

		///<value>The current position in the buffer.</value>
		protected int m_position = 0;

		///<summary>Constructor used to create a Byte Stream.</summary>
		///<param name="buffer">The byte array to read from.</param>
		internal ByteStream(byte[] buffer) { m_buffer = buffer; }

		///<summary>Reads a single byte from the buffer.</summary>
		///<returns>The byte read from the buffer.</returns>
		///<exception cref="System.IndexOutOfRangeException">Thrown when the number of bytes to read is greater than the number of bytes remaining in the buffer.</exception>
		internal byte ReadByte()
		{
			if (m_position >= m_buffer.Length) throw new System.IndexOutOfRangeException("End of stream reached");
			return m_buffer[m_position++];
		}

		///<summary>Reads a specified number of bytes from the buffer.</summary>
		///<param name="count">The number of bytes to read.</param>
		///<returns>The bytes read from the buffer.</returns>
		///<exception cref="System.IndexOutOfRangeException">Thrown when the number of bytes to read is greater than the number of bytes remaining in the buffer.</exception>
		internal byte[] ReadBytes(uint count)
		{
			if (m_position + count > m_buffer.Length) throw new System.IndexOutOfRangeException("End of stream reached");
			byte[] result = new byte[count];
			for (int i = 0; i < count; i++) result[i] = ReadByte();
			return result;
		}

		///<summary>Reads a short from the buffer.</summary>
		///<returns>The short read from the buffer.</returns>
		///<exception cref="System.IndexOutOfRangeException">Thrown when the number of bytes to read is greater than the number of bytes remaining in the buffer.</exception>
		internal short ReadShort() => BitConverter.ToInt16(ReadBytes(2), 0);

		///<summary>Reads an int from the buffer.</summary>
		///<returns>The int read from the buffer.</returns>
		///<exception cref="System.IndexOutOfRangeException">Thrown when the number of bytes to read is greater than the number of bytes remaining in the buffer.</exception>
		internal int ReadInt() => BitConverter.ToInt32(ReadBytes(4), 0);

		///<summary>Reads a bool from the buffer.</summary>
		///<returns>The bool read from the buffer.</returns>
		///<exception cref="System.IndexOutOfRangeException">Thrown when the number of bytes to read is greater than the number of bytes remaining in the buffer.</exception>
		internal bool ReadBool() => ReadByte() != 0;

		///<summary>Reads a float from the buffer.</summary>
		///<returns>The float read from the buffer.</returns>
		///<exception cref="System.IndexOutOfRangeException">Thrown when the number of bytes to read is greater than the number of bytes remaining in the buffer.</exception>
		internal float ReadFloat() => BitConverter.ToSingle(ReadBytes(4), 0);

		///<summary>Reads a double from the buffer.</summary>
		///<returns>The double read from the buffer.</returns>
		///<exception cref="System.IndexOutOfRangeException">Thrown when the number of bytes to read is greater than the number of bytes remaining in the buffer.</exception>
		internal double ReadDouble() => BitConverter.ToDouble(ReadBytes(8), 0);

		///<summary>Reads a string from the buffer.</summary>
		///<returns>The string read from the buffer.</returns>
		///<exception cref="System.IndexOutOfRangeException">Thrown when the number of bytes to read is greater than the number of bytes remaining in the buffer.</exception>
		///<exception cref="System.Text.DecoderFallbackException">Thrown when the string contains invalid characters.</exception>
		///<remarks>Strings are encoded as a short specifying the length of the string, followed by the string itself.</remarks>
		internal string ReadString() => Encoding.UTF8.GetString(ReadBytes((uint)ReadShort()));
	}

	///<summary>Record <c>EpochTime</c> is used to represent a time in the Unix Epoch.</summary>
	internal record EpochTime
	{
		///<value>The time represented by the Unix time as a <c>DateTime</c>.</value>
		public DateTime Time { get; private init; }

		///<value>The time represented by the Unix time as a <c>uint</c>.</value>
		public uint UnixTimeStamp { get; private init; }

		///<summary>Constructor used to create an Epoch Time.</summary>
		internal EpochTime(uint unix_time_stamp)
		{
			UnixTimeStamp = unix_time_stamp;
			Time = Utilities.UnixTimeStampToDateTime(unix_time_stamp);
		}
	}

	///<summary>Class <c>Logger</c> is used to log messages to the console.</summary>
	public static class Logger
	{
		///<value>Whether or not logging is enabled.</value>
		public static bool Enabled { get; set; } = true;

		///<summary>Logs a message to the console.</summary>
		///<param name="message">The message to log.</param>
		public static void Log(string message) { if (Enabled) Console.WriteLine("[LOG] " + message); }

		///<summary>Logs a warning to the console.</summary>
		///<param name="message">The message to log.</param>
		public static void Warn(string message) { if (Enabled) Console.WriteLine("[WARN] " + message); }

		///<summary>Logs an error to the console.</summary>
		///<param name="message">The message to log.</param>
		public static void Error(string message) { if (Enabled) Console.WriteLine("[ERROR] " + message); }

		///<summary>Logs a debug message to the console.</summary>
		///<param name="message">The message to log.</param>
		public static void Debug(string message) { if (Enabled) Console.WriteLine("[DEBUG] " + message); }

		///<summary>Logs info to the console.</summary>
		///<param name="message">The message to log.</param>
		public static void Info(string message) { if (Enabled) Console.WriteLine("[INFO] " + message); }
	}

	///<summary>Exception <c>AnmParsingException</c> is thrown when an error occurs while parsing an ANM file.</summary>
	public class AnmParsingException : Exception
	{
		///<summary>Constructor used to create an <c>AnmParsingException</c>.</summary>
		public AnmParsingException() { }

		///<summary>Constructor used to create an <c>AnmParsingException</c>.</summary>
		///<param name="message">The message to display.</param>
		public AnmParsingException(string message) : base(message) { }

		///<summary>Constructor used to create an <c>AnmParsingException</c>.</summary>
		///<param name="message">The message to display.</param>
		///<param name="inner">The inner exception.</param>
		public AnmParsingException(string message, Exception inner) : base(message, inner) { }
	}
}