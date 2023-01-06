using System.ComponentModel.DataAnnotations;
using AppleTv.Movie.Price.Tracker.Data;
using AppleTv.Movie.Price.Tracker.Domains.Movies.Models;
using AppleTv.Movie.Price.Tracker.Domains.Movies.Queries.GetMovies;
using AppleTv.Movie.Price.Tracker.Domains.Movies.Queries.LookupMovie;
using AppleTv.Movie.Price.Tracker.Domains.Movies.Queries.SearchMovies;
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

    /// <summary>
    /// Search movies on iTunes (Apple TV movies)
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("search")]
    public async Task<ActionResult<MovieSearchPagedModel>> Search([FromQuery] SearchMoviesQuery query)
    {
        var result = await mediator.Send(query);

        return Ok(result);
    }

    [HttpGet]
    [Route("lookup/{country}/{trackId:long}")]
    public async Task<ActionResult<ITunesSearchResultItemModel>> Lookup([FromRoute] string country, [FromRoute] long trackId, [FromQuery] string language)
    {
        LookupMovieQuery query = new(trackId, country)
        {
            Language = language,
        };
        var result = await mediator.Send(query);

        return Ok(result);
    }

    [HttpPost]
    [Route("track")]
    public async Task<IActionResult> Track([FromBody] TrackModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        var result = await iTunesSearchService.LookupMovieAsync(model.Id, model.Country, model.Language);

        if (result == null)
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

            movie.ArtistName = result.ArtistName;
            movie.ArtworkUrl100 = result.ArtworkUrl100;
            movie.ArtworkUrl30 = result.ArtworkUrl30;
            movie.ArtworkUrl60 = result.ArtworkUrl60;
            movie.ArtworkUrlBase = result.ArtworkUrlBase;
            movie.CollectionPrice = result.CollectionPrice;
            movie.CollectionHdPrice = result.CollectionHdPrice;
            movie.TrackPrice = result.TrackPrice;
            movie.TrackHdPrice = result.TrackHdPrice;
            movie.TrackRentalPrice = result.TrackRentalPrice;
            movie.TrackHdRentalPrice = result.TrackHdRentalPrice;

            await appDbContext.SaveChangesAsync();

            logger.LogInformation("Movie updated: {title} ({trackId};{country})", movie.TrackName, movie.TrackId, movie.Country);
        }
        else
        {

            movie = mapper.Map<Entities.Movie>(result);

            var added = appDbContext.Movies.Add(movie);

            movieId = added.Entity.Id;

            await appDbContext.SaveChangesAsync();

            logger.LogInformation("Movie added: {title} ({trackId};{country})", movie.TrackName, movie.TrackId, movie.Country);
        }

        if (result.CollectionId > 0)
        {
            Guid collectionId;

            var collection = await appDbContext.Collections
                .Where(x => x.CollectionId == result.CollectionId)
                .FirstOrDefaultAsync();

            if (collection == null)
            {
                collection = new()
                {
                    CollectionArtistId = result.CollectionArtistId,
                    CollectionArtistViewUrl = result.CollectionArtistViewUrl,
                    CollectionCensoredName = result.CollectionCensoredName,
                    CollectionHdPrice = result.CollectionHdPrice,
                    CollectionId = result.CollectionId,
                    CollectionName = result.CollectionName,
                    CollectionPrice = result.CollectionPrice,
                    CollectionViewUrl = result.CollectionViewUrl,
                };

                var addedCollection = appDbContext.Collections.Add(collection);
                collectionId = addedCollection.Entity.Id;
            }
            else
            {
                collection.CollectionArtistId = result.CollectionArtistId;
                collection.CollectionArtistViewUrl = result.CollectionArtistViewUrl;
                collection.CollectionCensoredName = result.CollectionCensoredName;
                collection.CollectionHdPrice = result.CollectionHdPrice;
                collection.CollectionId = result.CollectionId;
                collection.CollectionName = result.CollectionName;
                collection.CollectionPrice = result.CollectionPrice;
                collection.CollectionViewUrl = result.CollectionViewUrl;

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
