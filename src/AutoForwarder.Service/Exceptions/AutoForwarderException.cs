namespace AutoForwarder.Service.Exceptions;

public class AutoForwarderException : Exception
{
    public int statusCode { get; set; }
    public AutoForwarderException(int statusCode,string message) : base(message)
    {
        this.statusCode = statusCode;
    }
}
