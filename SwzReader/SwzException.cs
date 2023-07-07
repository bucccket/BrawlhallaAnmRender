namespace BrawlhallaANMReader.Swz;

public class SwzException : Exception
{
    public SwzException(string message)
        : base(message)
    {
    }

    public SwzException(string message, Exception ex)
        : base(message, ex)
    {
    }
}

