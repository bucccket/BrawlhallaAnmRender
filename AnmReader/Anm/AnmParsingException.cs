namespace BrawlhallaANMReader.Anm
{
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
