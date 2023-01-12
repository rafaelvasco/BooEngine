namespace Boo.Common;

public class BooException : Exception
{
    public BooException() {}

    public BooException(string? message): base(message) {}

    public BooException(string? message, Exception? innerException) : base(message, innerException) {}
}
