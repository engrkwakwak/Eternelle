using Eternelle.Common.Domain;

namespace Eternelle.Common.Application.Exceptions;

public sealed class EternelleException : Exception
{
    public EternelleException(string requestName, Error? error = default, Exception? innerException = default)
        : base("Application exception", innerException)
    {
        RequestName = requestName;
        Error = error;
    }

    public string RequestName { get; }

    public Error? Error { get; }
}
