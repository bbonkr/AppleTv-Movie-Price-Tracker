using System;
namespace AppleTv.Movie.Price.Tracker.App.Options;

public class CorsOptions
{
    public const string Name = "Cors";
    public const string Separator = ";";

    public string Origins { get; set; } = "*";

    public string Headers { get; set; } = "*";

    public string Methods { get; set; } = "*";

    public bool AllowsAnyOrigins => GetOrigins().Contains("*");

    public bool AllowsAnyHeaders => GetHeaders().Contains("*");

    public bool AllowsAnyMethods => GetMethods().Contains("*");

    public IEnumerable<string> GetOrigins()
    {
        if (string.IsNullOrWhiteSpace(Origins))
        {
            return new List<string>();
        }

        return Origins.Split(Separator, StringSplitOptions.RemoveEmptyEntries);
    }

    public IEnumerable<string> GetHeaders()
    {
        if (string.IsNullOrWhiteSpace(Headers))
        {
            return new List<string>();
        }

        return Headers.Split(Separator, StringSplitOptions.RemoveEmptyEntries);
    }

    public IEnumerable<string> GetMethods()
    {
        if (string.IsNullOrWhiteSpace(Methods))
        {
            return new List<string>();
        }

        return Methods.Split(Separator, StringSplitOptions.RemoveEmptyEntries);
    }
}

