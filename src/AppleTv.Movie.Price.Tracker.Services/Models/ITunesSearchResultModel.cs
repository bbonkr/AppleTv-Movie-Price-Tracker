using System;
namespace AppleTv.Movie.Price.Tracker.Services.Models;

public class ITunesSearchResultModel
{
    public int ResultCount { get; set; }

    public IEnumerable<ITunesSearchResultItemModel> Results { get; set; } = Enumerable.Empty<ITunesSearchResultItemModel>();
}

