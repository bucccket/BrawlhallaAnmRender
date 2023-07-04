namespace BrawlhallaANMReader.Steam
{
    public class VDFException : Exception
    {
        public VDFException(string message)
            : base(message)
        {
        }

        public VDFException(string message, Exception ex)
            : base(message, ex)
        {
        }
    }
}
