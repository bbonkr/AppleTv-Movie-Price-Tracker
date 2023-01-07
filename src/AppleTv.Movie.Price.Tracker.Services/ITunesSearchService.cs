using System.Text;
using System.Text.Json;
using System.Threading;
using AppleTv.Movie.Price.Tracker.Services.Models;
using kr.bbon.Core.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AppleTv.Movie.Price.Tracker.Services;

public class ITunesSearchService
{
    private const string SearchUrl = "https://itunes.apple.com/search";
    private const string LookupUrl = "https://itunes.apple.com/{0}/lookup";

    public const uint LIMIT = 25;

    public ITunesSearchService(
        HttpClient client,
        IOptionsMonitor<JsonSerializerOptions> jsonSerializerOptionsAccessor,
        ILogger<ITunesSearchService> logger)
    {
        this.client = client;
        this.logger = logger;
        jsonSerializerOptions = jsonSerializerOptionsAccessor.CurrentValue ?? throw new ArgumentNullException(nameof(jsonSerializerOptionsAccessor));
    }

    public async Task<ITunesSearchResultModel> SearchMoviesAsync(string term, string storeCountry, string language, int limit = (int)LIMIT, CancellationToken cancellationToken = default)
    {
        var storeCountryCode = storeCountry.Trim().ToLower();
        var languageCode = language.Trim().Replace('-', '_');

        List<string> queries = new() {
            $"term={term.Trim().Replace(' ', '+')}",
            $"country={storeCountryCode}",
            $"lang={languageCode}",
            $"media=movie",
            $"entity=movie",
            $"attribute=movieTerm",
        };


        var url = GenerateUrl(SearchUrl, queries);

        var response = await client.GetAsync(url, cancellationToken: cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = JsonSerializer.Deserialize<ITunesSearchResultModel>(json, jsonSerializerOptions) ?? new ITunesSearchResultModel { ResultCount = 0 };

            foreach (var item in result.results)
            {
                item.CountryCode = storeCountryCode;
                item.LanguageCode = languageCode;
            }

            return result;
        }
        else
        {
            logger.LogWarning("[{className}][{methodName}]: ({statusCode}) {reasonPhrase}",
                nameof(ITunesSearchService),
                nameof(SearchMoviesAsync),
                response.StatusCode,
                response.ReasonPhrase);
            throw new ApiException(response.StatusCode, response.ReasonPhrase);
        }
    }

    public async Task<ITunesSearchResultItemModel?> LookupMovieAsync(long id, string storeCountry, string language, CancellationToken cancellationToken = default)
    {
        var storeCountryCode = storeCountry.Trim().ToLower();
        var languageCode = language.Trim().Replace('-', '_');

        List<string> queries = new()
        {
            $"id={id}",
         };

        var url = GenerateUrl(string.Format(LookupUrl, storeCountry.Trim().ToLower()), queries);

        var response = await client.GetAsync(url, cancellationToken: cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = JsonSerializer.Deserialize<ITunesSearchResultModel>(json, jsonSerializerOptions) ?? new ITunesSearchResultModel { ResultCount = 0 };

            foreach (var item in result.results)
            {
                item.CountryCode = storeCountryCode;
                item.LanguageCode = languageCode;
            }

            return result.results.FirstOrDefault();
        }
        else
        {
            logger.LogWarning("[{className}][{methodName}]: ({statusCode}) {reasonPhrase}",
                nameof(ITunesSearchService),
                nameof(LookupMovieAsync),
                response.StatusCode,
                response.ReasonPhrase);

            throw new ApiException(response.StatusCode, response.ReasonPhrase);
        }
    }

    private string GenerateUrl(string baseUrl, IEnumerable<string> queries)
    {
        StringBuilder urlBuilder = new();
        urlBuilder.Append(baseUrl);

        if (queries.Any())
        {
            urlBuilder.Append("?");

            foreach (var query in queries)
            {
                urlBuilder.AppendFormat("&{0}", query);
            }
        }

        return urlBuilder.ToString();
    }

    private readonly HttpClient client;
    private readonly JsonSerializerOptions jsonSerializerOptions;
    private readonly ILogger logger;
}


