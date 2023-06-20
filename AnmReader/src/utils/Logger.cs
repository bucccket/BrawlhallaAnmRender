namespace BrawlhallaANMReader.utils
{
	///<summary>Class <c>Logger</c> is used to log messages to the console.</summary>
	///<remarks>Logging to the standard output resets the console colours after logging.</remarks>
	public static class Logger
	{
		///<value>Whether or not logging to the standard output is enabled.</value>
		public static bool ConsoleEnable { get; set; } = true;

		///<value>Whether or not logging to a file is enabled.</value>
		public static bool FileEnable { get; set; } = false;

		///<value>The path to the log file.</value>
		public static string FilePath { get; set; } = "./log.log";

		///<value>The format of the timestamp.</value>
		public static string TimestampFormat { get; set; } = "[hh:mm:ss tt]";

		///<summary>Initializes the logger.</summary>
		static Logger()
		{
			if (File.Exists(FilePath)) File.Delete(FilePath);
			File.Create(FilePath).Close();
		}

		///<summary>Logs a message to the console.</summary>
		///<param name="prefix">The prefix to log.</param>
		///<param name="message">The message to log.</param>
		///<param name="colour">The colour to log the message in.</param>
		private static void Out(string message, string prefix = "", ConsoleColor colour = ConsoleColor.White)
		{
			DateTime now = DateTime.Now;
			if (ConsoleEnable)
			{
				Console.Write(now.ToString(TimestampFormat + '\t'));
				if (prefix != "")
				{
					Console.ForegroundColor = (colour == ConsoleColor.White ? ConsoleColor.Black : ConsoleColor.White);
					Console.BackgroundColor = colour;
					Console.Write(prefix);
					Console.BackgroundColor = ConsoleColor.Black;
					Console.Write('\t');
				}
				Console.ForegroundColor = colour;
				Console.WriteLine(message);
				Console.ResetColor();
			}
			if (FileEnable)
			{
				if (!File.Exists(FilePath)) File.Create(FilePath);
				using (StreamWriter sw = File.AppendText(FilePath))
				{
					sw.Write(now.ToString(TimestampFormat + '\t'));
					if (prefix != "")
					{
						sw.Write(prefix);
						sw.Write('\t');
					}
					sw.WriteLine(message);
					sw.Close();
				}
			}
		}

		///<summary>Logs a message.</summary>
		///<param name="message">The message to log.</param>
		public static void Log(string message) { Out(message, "[LOG]"); }

		///<summary>Logs a warning.</summary>
		///<param name="message">The message to log.</param>
		public static void Warn(string message) { Out(message, "[WARN]", ConsoleColor.DarkYellow); }

		///<summary>Logs an error.</summary>
		///<param name="message">The message to log.</param>
		public static void Error(string message) { Out(message, "[ERROR]", ConsoleColor.DarkRed); }

		///<summary>Logs a debug message.</summary>
		///<param name="message">The message to log.</param>
		public static void Debug(string message) { Out(message, "[DEBUG]", ConsoleColor.Green); }

		///<summary>Logs info.</summary>
		///<param name="message">The message to log.</param>
		public static void Info(string message) { Out(message, "[INFO]", ConsoleColor.DarkBlue); }
	}
}
