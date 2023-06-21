namespace BrawlhallaANMReader.utils
////TODO: better name
{
	///<summary>Class <c>NullReferenceTypeException</c> is used to throw an exception when a search for a type returns null.</summary>
	public class NullReferenceTypeException : Exception
	{
		///<summary><c>NullReferenceTypeException</c> is thrown when a search for a type returns null.</summary>
		public NullReferenceTypeException() { }

		///<summary><c>NullReferenceTypeException</c> is thrown when a search for a type returns null.</summary>
		public NullReferenceTypeException(string message) : base(message) { }

		///<summary><c>NullReferenceTypeException</c> is thrown when a search for a type returns null.</summary>
		public NullReferenceTypeException(string message, Exception inner) : base(message, inner) { }
	}
}