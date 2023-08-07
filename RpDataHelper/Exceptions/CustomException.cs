using System.Net;

namespace RpDataHelper.Exceptions;

public class CustomException : Exception
{
    public HttpStatusCode StatusCode { get; set; }

    public CustomException(string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest) : base(message)
    {
        StatusCode = statusCode;
    }
}