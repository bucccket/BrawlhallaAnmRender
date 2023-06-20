namespace BrawlhallaANMReader.utils
{
	////TODO: add a timestamp to the log
	////TODO: add logging to a file
	////TODO: add fancy colours
	///<summary>Class <c>Logger</c> is used to log messages to the console.</summary>
	public static class Logger
	{
		///<value>Whether or not logging is enabled.</value>
		public static bool Enabled { get; set; } = true;

		///<summary>Logs a message to the console.</summary>
		///<param name="message">The message to log.</param>
		private static void Out(object message) { if (Enabled) Console.WriteLine(message); }

		///<summary>Logs a message.</summary>
		///<param name="message">The message to log.</param>
		public static void Log(string message) { Out("[LOG] " + message); }

		///<summary>Logs a warning.</summary>
		///<param name="message">The message to log.</param>
		public static void Warn(string message) { Out("[WARN] " + message); }

		///<summary>Logs an error.</summary>
		///<param name="message">The message to log.</param>
		public static void Error(string message) { Out("[ERROR] " + message); }

		///<summary>Logs a debug message.</summary>
		///<param name="message">The message to log.</param>
		public static void Debug(string message) { Out("[DEBUG] " + message); }

		///<summary>Logs info.</summary>
		///<param name="message">The message to log.</param>
		public static void Info(string message) { Out("[INFO] " + message); }
	}
}
