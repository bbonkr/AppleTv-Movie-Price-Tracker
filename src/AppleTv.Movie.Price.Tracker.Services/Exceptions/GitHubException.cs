using System.Net;

namespace AppleTv.Movie.Price.Tracker.Services.Exceptions;

public class GitHubException : Exception
{
    public GitHubException(HttpStatusCode statusCode, GitHubError error) : base(error.Message)
    {
        StatusCode = statusCode;
        Error = error;
    }


    public HttpStatusCode StatusCode { get; private set; }

    public GitHubError Error { get; private set; }
}
