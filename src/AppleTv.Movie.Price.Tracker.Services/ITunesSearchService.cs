using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using AppleTv.Movie.Price.Tracker.Services.Models;
using kr.bbon.Core.DataSets;
using kr.bbon.Core.Exceptions;
using Microsoft.Extensions.Logging;

namespace AppleTv.Movie.Price.Tracker.Services;

public class ITunesSearchService
{
    private const string SearchUrl = "https://itunes.apple.com/search";
    private const string LookupUrl = "https://itunes.apple.com/{0}/lookup";

    public ITunesSearchService(HttpClient client, ILogger<ITunesSearchService> logger)
    {
        this.client = client;
        this.logger = logger;
    }

    public async Task<ITunesSearchResultModel> SearchAsync(string term, string storeCountry, string language, int limit = 25, CancellationToken cancellationToken = default)
    {
        var storeCountryCode = storeCountry.Trim().ToLower();
        var languageCode = language.Trim().Replace('-', '_');

        List<string> queries = new();
        queries.Add($"term={term.Trim().Replace(' ', '+')}");
        queries.Add($"country={storeCountryCode}");
        queries.Add($"lang={languageCode}");
        queries.Add($"media=movie");
        queries.Add($"entity=movie");
        queries.Add($"attribute=movieArtistTerm");

        var url = GenerateUrl(SearchUrl, queries);

        var response = await client.GetAsync(url);

        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = JsonSerializer.Deserialize<ITunesSearchResultModel>(json) ?? new ITunesSearchResultModel { ResultCount = 0 };

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
                nameof(SearchAsync),
                response.StatusCode,
                response.ReasonPhrase);
            throw new ApiException(response.StatusCode, response.ReasonPhrase);
        }
    }

    public async Task<ITunesSearchResultModel> LookupAsync(long id, string storeCountry, string language, CancellationToken cancellationToken = default)
    {
        var storeCountryCode = storeCountry.Trim().ToLower();
        var languageCode = language.Trim().Replace('-', '_');

        List<string> queries = new();
        queries.Add($"id={id}");

        var url = GenerateUrl(string.Format(SearchUrl, storeCountry.Trim().ToLower()), queries);

        var response = await client.GetAsync(url);

        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = JsonSerializer.Deserialize<ITunesSearchResultModel>(json) ?? new ITunesSearchResultModel { ResultCount = 0 };

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
                nameof(LookupAsync),
                response.StatusCode,
                response.ReasonPhrase);

            throw new ApiException(response.StatusCode, response.ReasonPhrase);
        }
    }

    private string GenerateUrl(string baseUrl, IEnumerable<string> queries)
    {
        StringBuilder urlBuilder = new StringBuilder();
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
    private readonly ILogger logger;
}


