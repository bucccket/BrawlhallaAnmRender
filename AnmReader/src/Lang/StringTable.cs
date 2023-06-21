using System.Text;
using System.Text.RegularExpressions;
using BrawlhallaANMReader.utils;

namespace BrawlhallaANMReader.Lang
{
	///<summary> Class <c>StringTable</c> is used to parse and store the string table from the game's <c>language.x.bin</c> files.</summary>
	public static class StringTable
	{
		///<value>Dictionary used to store the string table.</value>
		private static readonly Dictionary<string, string?[]> s_string_table = new();

		///<value>Number of languages parsed from the language files.</value>
		public static uint LanguageCount { get; private set; }

		///<value>Pattern used to match the language files.</value>		
		private static readonly string s_language_bin_filename = @"language.*.bin";

		///<value>Regex used to match the language files.</value>
		private static readonly string s_language_bin_regex = @"language\.\d+\.bin";

		///<value>Header used to identify the language files.</value>
		private static readonly byte[] s_language_bin_header = { 0x00, 0x00, 0x21, 0x90 };

		///<summary>Loads the language files from the specified directory.</summary>
		///<param name="path">The path to the directory containing the language files.</param>
		///<exception cref="FileNotFoundException">Thrown when no valid language files are found in the specified directory.</exception>
		///<remarks>
		///<para>Language files must be named <c>language.x.bin</c>, where <c>x</c> is the Language ID.<br/>
		///This method will clear the string table before loading the language files.</para>
		///</remarks>
		public static void LoadLanguageBins(string path)
		{
			if (path is null)
			{
				Logger.Error("No language files found in the specified directory.");
				throw new FileNotFoundException("No language files found in the specified directory.");
			}
			ClearStringTable();
			string[] files = Directory.GetFiles(path, s_language_bin_filename, SearchOption.TopDirectoryOnly);
			if (files.Length == 0)
			{
				Logger.Error("No language files found in the specified directory.");
				throw new FileNotFoundException("No language files found in the specified directory.");
			}
			List<string> valid_files = new();
			foreach (string file in files) if (Regex.IsMatch(file, s_language_bin_regex)) valid_files.Add(file);
			if (valid_files.Count == 0)
			{
				Logger.Error("No valid language files found in the specified directory.");
				throw new FileNotFoundException("No valid language files found in the specified directory.");
			}
			LanguageCount = (uint)valid_files.Count;
			foreach (string file in valid_files)
			{
				uint language_id = uint.Parse(file.Split('.')[1]);
				if (language_id == 0) continue;
				ParseBinFile(file, language_id);
			}
		}

		///<summary>Loads a single language file from the path.</summary>
		///<param name="path">The path to the language file.</param>
		///<exception cref="FileNotFoundException">Thrown when the specified language file is not found.</exception>
		///<remarks>
		///<para>The Language ID is always assumed to be <c>1</c><br/>
		///This method will clear the string table before loading the language file.</para>
		///</remarks>
		public static void LoadLanguageBin(string path)
		{
			if (path is null)
			{
				Logger.Error("Invalid path to language file.");
				throw new FileNotFoundException("Invalid path to language file.");
			}
			ClearStringTable();
			LanguageCount = 1;
			ParseBinFile(path);
		}

		///<summary>Parses the specified language file into the string table.</summary>
		///<param name="path">The path to the language file.</param>
		///<exception cref="FileNotFoundException">Thrown when the specified language file is not found.</exception>
		private static void ParseBinFile(string path, uint language_id = 1)
		{
			using FileStream data = new(path, FileMode.Open, FileAccess.Read);
			using MemoryStream memory_stream = new();
			byte[] _ = new byte[4];
			data.Read(_, 0, 4);
			data.CopyTo(memory_stream);
			byte[] buffer = memory_stream.ToArray();
			ReadStringTable(Compression.InflateBuffer(buffer), language_id);
		}

		///<summary>Clears the string table.</summary>
		public static void ClearStringTable()
		{
			s_string_table.Clear();
			LanguageCount = 0;
		}

		///<summary>Parses the specified byte array into the string table.</summary>
		///<param name="table">Byte array containing the string table.</param>
		///<param name="language_id">Language ID of the string table.</param>
		private static void ReadStringTable(byte[] table, uint language_id)
		{
			uint index = 4;
			while (index < table.Length)
			{
				byte[] key_length_bytes = { table[index + 1], table[index] };
				ushort key_length = BitConverter.ToUInt16(key_length_bytes, 0);
				index += 2;
				string key = Encoding.UTF8.GetString(table, (int)index, key_length);
				index += key_length;
				byte[] value_length_bytes = { table[index + 1], table[index] };
				ushort value_length = BitConverter.ToUInt16(value_length_bytes, 0);
				index += 2;
				string value = Encoding.UTF8.GetString(table, (int)index, value_length);
				index += value_length;
				AddString(key, value, language_id);
			}
		}

		///<summary>Adds or modifies a string in the string table.</summary>
		///<param name="key">Key of the string to add or modify.</param>
		///<param name="value">Value of the string to add or modify.</param>
		///<param name="language_id">Language ID of the string to add or modify.</param>
		///<exception cref="ArgumentException">Thrown when the Language ID is <c>0</c>.</exception>
		///<remarks>If the specified string is not found in the string table, it will be added.</remarks>
		private static void AddString(string key, string value, uint language_id)
		{
			if (language_id == 0)
			{
				Logger.Error("Language ID cannot be 0.");
				throw new ArgumentException("Language ID cannot be 0.");
			}
			if (s_string_table.ContainsKey(key)) s_string_table[key][language_id] = value;
			else
			{
				s_string_table.Add(key, new string?[LanguageCount + 1]);
				s_string_table[key][language_id] = value;
			}
		}

		///<summary>Returns the value of a string in the string table.</summary>
		///<param name="key">Key of the string to return.</param>
		///<param name="language_id">Language ID of the string to return.  Default value is <c>1</c> (English).</param>
		///<returns>The value of the string in the string table.</returns>
		///<remarks>
		///<para>If the specified string is not found for the specified language, the English string will be returned.<br/>
		///If the specified string is not found for the specified language or English, the key will be returned.<br/>
		///If the specified Language ID is <c>0</c>, the key will be returned.<br/>
		///If the specified key is not found, it will be echoed back.<br/>
		///This return behaviour mimics the that of the game.</para>
		///</remarks>
		public static string GetString(string key, uint language_id = 1) => s_string_table.ContainsKey(key) && language_id != 0 ? s_string_table?[key]?[language_id] ?? s_string_table?[key]?[1] ?? key : key;

		///<summary>Returns the value of a string in the string table.</summary>
		///<param name="key">Key of the string to return.</param>
		///<param name="language">Language of the string to return.</param>
		///<returns>The value of the string in the string table.</returns>
		///<remarks>
		///<para>If the specified string is not found for the specified language, the English string will be returned.<br/>
		///If the specified string is not found for the specified language or English, the key will be returned.<br/>
		///If the specified Language ID is <c>0</c>, the key will be returned.<br/>
		///If the specified key is not found, it will be echoed back.<br/>
		///This return behaviour mimics the that of the game.</para>
		///</remarks>
		public static string GetString(string key, LanguageType language) => GetString(key, language.LanguageID);
	}
}
