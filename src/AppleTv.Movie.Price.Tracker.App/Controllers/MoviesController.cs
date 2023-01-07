using AppleTv.Movie.Price.Tracker.Domains.Models;
using AppleTv.Movie.Price.Tracker.Domains.Movies.Commands.TrackMovie;
using AppleTv.Movie.Price.Tracker.Domains.Movies.Commands.UntrackMovie;
using AppleTv.Movie.Price.Tracker.Domains.Movies.Models;
using AppleTv.Movie.Price.Tracker.Domains.Movies.Queries.GetMovieById;
using AppleTv.Movie.Price.Tracker.Domains.Movies.Queries.GetMovies;
using AppleTv.Movie.Price.Tracker.Domains.Movies.Queries.LookupMovie;
using AppleTv.Movie.Price.Tracker.Domains.Movies.Queries.SearchMovies;
using AppleTv.Movie.Price.Tracker.Services.Models;
using kr.bbon.AspNetCore;
using kr.bbon.AspNetCore.Mvc;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppleTv.Movie.Price.Tracker.App.Controllers;

[Authorize]
[ApiController]
[Area(DefaultValues.AreaName)]
[Route(DefaultValues.RouteTemplate)]
[ApiVersion(DefaultValues.ApiVersion)]
[Produces(Constants.RESPONSE_MEDIA_TYPE)]
public class MoviesController : ApiControllerBase
{
    public MoviesController(IMediator mediator, ILogger<MoviesController> logger)
    {
        this.mediator = mediator;
        this.logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<MoviesPagedModel>> GetMovies([FromQuery] GetMoviesQuery query)
    {
        var result = await mediator.Send(query);

        return Ok(result);
    }

    [HttpGet]
    [Route("{id:guid}")]
    public async Task<ActionResult<MovieModel>> GetMovie([FromRoute] Guid id)
    {
        GetMovieByIdQuery query = new(id);
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
    public async Task<ActionResult<MovieModel>> Track([FromBody] TrackMovieCommand command)
    {
        var result = await mediator.Send(command);

        return Accepted(result);
    }

    [HttpDelete()]
    [Route("{movieId:guid}/untrack")]
    public async Task<ActionResult<bool>> Untrack([FromRoute] Guid movieId)
    {
        UntrackMovieCommand command = new() { Id = movieId };

        var result = await mediator.Send(command);

        return Accepted(result);
    }

    private readonly IMediator mediator;
    private readonly ILogger logger;
}
