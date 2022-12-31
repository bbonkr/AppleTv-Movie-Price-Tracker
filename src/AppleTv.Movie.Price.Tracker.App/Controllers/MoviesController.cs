using System.ComponentModel.DataAnnotations;
using AppleTv.Movie.Price.Tracker.Data;
using AppleTv.Movie.Price.Tracker.Domains.Movies.Models;
using AppleTv.Movie.Price.Tracker.Domains.Movies.Queries.GetMovies;
using AppleTv.Movie.Price.Tracker.Services;
using AppleTv.Movie.Price.Tracker.Services.Models;
using AutoMapper;
using kr.bbon.AspNetCore;
using kr.bbon.AspNetCore.Mvc;
using kr.bbon.Core.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppleTv.Movie.Price.Tracker.App.Controllers;

[Authorize]
[ApiController]
[Area(DefaultValues.AreaName)]
[Route(DefaultValues.RouteTemplate)]
[ApiVersion(DefaultValues.ApiVersion)]
[Produces(Constants.RESPONSE_MEDIA_TYPE)]
public class MoviesController : ApiControllerBase
{
    public MoviesController(IMediator mediator, AppDbContext appDbContext, ITunesSearchService iTunesSearchService, IMapper mapper, ILogger<MoviesController> logger)
    {
        this.mediator = mediator;
        this.appDbContext = appDbContext;
        this.iTunesSearchService = iTunesSearchService;
        this.mapper = mapper;
        this.logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<MoviesPagedModel>> GetMovies([FromQuery] GetMoviesQuery query)
    {
        var result = await mediator.Send(query);

        return Ok(result);
    }



    [HttpGet]
    [Route("search")]
    public async Task<ActionResult<IEnumerable<ITunesSearchResultItemModel>>> Search([FromQuery] string term, [FromQuery] string country = "kr", [FromQuery] string language = "ko_kr")
    {
        if (string.IsNullOrWhiteSpace(term))
        {
            return BadRequest();
        }

        if (string.IsNullOrWhiteSpace(country))
        {
            return BadRequest();
        }

        if (string.IsNullOrWhiteSpace(language))
        {
            return BadRequest();
        }

        var result = await iTunesSearchService.SearchAsync(term, country, language);

        return Ok(result.results);
    }

    [HttpGet]
    [Route("lookup/{country}/{trackId:int}")]
    public async Task<ActionResult<IEnumerable<ITunesSearchResultItemModel>>> Lookup([FromRoute] long trackId, [FromRoute] string country = "kr", [FromQuery] string language = "ko_kr")
    {
        if (string.IsNullOrWhiteSpace(country))
        {
            return BadRequest();
        }

        if (trackId < 1)
        {
            return BadRequest();
        }


        var result = await iTunesSearchService.LookupAsync(trackId, country, language);

        return Ok(result.results);
    }



    [HttpPost]
    [Route("track")]
    public async Task<IActionResult> Track([FromBody] TrackModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        var result = await iTunesSearchService.LookupAsync(model.Id, model.Country, model.Language);

        var item = result.results.FirstOrDefault();
        if (item == null)
        {
            throw new ApiException(System.Net.HttpStatusCode.NotFound);
        }

        var movie = await appDbContext.Movies
            .Where(x => x.TrackId == model.Id && x.CountryCode == model.Country && x.LanguageCode == model.Language)
            .FirstOrDefaultAsync();

        Guid movieId;

        if (movie != null)
        {
            movieId = movie.Id;

            movie.ArtistName = item.ArtistName;
            movie.ArtworkUrl100 = item.ArtworkUrl100;
            movie.ArtworkUrl30 = item.ArtworkUrl30;
            movie.ArtworkUrl60 = item.ArtworkUrl60;
            movie.ArtworkUrlBase = item.ArtworkUrlBase;
            movie.CollectionPrice = item.CollectionPrice;
            movie.CollectionHdPrice = item.CollectionHdPrice;
            movie.TrackPrice = item.TrackPrice;
            movie.TrackHdPrice = item.TrackHdPrice;
            movie.TrackRentalPrice = item.TrackRentalPrice;
            movie.TrackHdRentalPrice = item.TrackHdRentalPrice;

            await appDbContext.SaveChangesAsync();

            logger.LogInformation("Movie updated: {title} ({trackId};{country})", movie.TrackName, movie.TrackId, movie.Country);
        }
        else
        {

            movie = mapper.Map<Entities.Movie>(item);

            var added = appDbContext.Movies.Add(movie);

            movieId = added.Entity.Id;

            await appDbContext.SaveChangesAsync();

            logger.LogInformation("Movie added: {title} ({trackId};{country})", movie.TrackName, movie.TrackId, movie.Country);
        }

        if (item.CollectionId > 0)
        {
            Guid collectionId;

            var collection = await appDbContext.Collections
                .Where(x => x.CollectionId == item.CollectionId)
                .FirstOrDefaultAsync();

            if (collection == null)
            {
                collection = new()
                {
                    CollectionArtistId = item.CollectionArtistId,
                    CollectionArtistViewUrl = item.CollectionArtistViewUrl,
                    CollectionCensoredName = item.CollectionCensoredName,
                    CollectionHdPrice = item.CollectionHdPrice,
                    CollectionId = item.CollectionId,
                    CollectionName = item.CollectionName,
                    CollectionPrice = item.CollectionPrice,
                    CollectionViewUrl = item.CollectionViewUrl,
                };

                var addedCollection = appDbContext.Collections.Add(collection);
                collectionId = addedCollection.Entity.Id;
            }
            else
            {
                collection.CollectionArtistId = item.CollectionArtistId;
                collection.CollectionArtistViewUrl = item.CollectionArtistViewUrl;
                collection.CollectionCensoredName = item.CollectionCensoredName;
                collection.CollectionHdPrice = item.CollectionHdPrice;
                collection.CollectionId = item.CollectionId;
                collection.CollectionName = item.CollectionName;
                collection.CollectionPrice = item.CollectionPrice;
                collection.CollectionViewUrl = item.CollectionViewUrl;

                collectionId = collection.Id;
            }

            await appDbContext.SaveChangesAsync();

            var collectionMovie = await appDbContext.CollectionMovies
                .Where(x => x.MovieId == movieId && x.CollectionId == collectionId)
                .FirstOrDefaultAsync();

            if (collectionMovie == null)
            {
                appDbContext.CollectionMovies.Add(new()
                {
                    CollectionId = collectionId,
                    MovieId = movieId,
                });

                await appDbContext.SaveChangesAsync();
            }
        }

        return Accepted();
    }

    [HttpDelete()]
    [Route("{movieId:guid}/untrack")]
    public async Task<IActionResult> Untrack([FromRoute] Guid movieId)
    {
        var movie = await appDbContext.Movies.Where(x => x.Id == movieId).FirstOrDefaultAsync();

        if (movie == null)
        {
            return NotFound();
        }

        var collectionMovies = appDbContext.CollectionMovies
            .Where(x => x.MovieId == movie.Id);

        foreach (var item in collectionMovies)
        {
            appDbContext.CollectionMovies.Remove(item);
        }

        appDbContext.Movies.Remove(movie);

        await appDbContext.SaveChangesAsync();

        return Accepted();
    }

    private readonly IMediator mediator;
    private readonly AppDbContext appDbContext;
    private readonly ITunesSearchService iTunesSearchService;
    private readonly IMapper mapper;
    private readonly ILogger logger;
}


public class TrackModel
{
    [Required]
    public long Id { get; set; }
    [Required]
    public string Country { get; set; } = "kr";
    [Required]
    public string Language { get; set; } = "ko_kr";
}
