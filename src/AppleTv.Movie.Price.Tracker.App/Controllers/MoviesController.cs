using System.ComponentModel.DataAnnotations;
using AppleTv.Movie.Price.Tracker.Data;
using AppleTv.Movie.Price.Tracker.Services;
using AppleTv.Movie.Price.Tracker.Services.Models;
using AutoMapper;
using kr.bbon.AspNetCore;
using kr.bbon.AspNetCore.Mvc;
using kr.bbon.EntityFrameworkCore.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace AppleTv.Movie.Price.Tracker.App.Controllers;

[ApiController]
[Area(DefaultValues.AreaName)]
[Route(DefaultValues.RouteTemplate)]
[ApiVersion(DefaultValues.ApiVersion)]
public class MoviesController : ApiControllerBase
{

    public MoviesController(AppDbContext appDbContext, ITunesSearchService iTunesSearchService, IMapper mapper, ILogger<MoviesController> logger)
    {
        this.appDbContext = appDbContext;
        this.iTunesSearchService = iTunesSearchService;
        this.mapper = mapper;
        this.logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Entities.Movie>>> GetMovies([FromQuery] int page = 1, [FromQuery] int limit = 10, [FromQuery] string keyword = "")
    {
        var query = appDbContext.Movies.AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(x => x.TrackName.Contains(keyword) || x.ArtistName.Contains(keyword) || x.CollectionName.Contains(keyword));
        }

        var result = await query.OrderByDescending(x => x.ReleaseDate)
            .ToPagedModelAsync(page, limit);

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
            return NotFound();
        }

        var movie = appDbContext.Movies.Where(x => x.TrackId == model.Id).FirstOrDefault();

        if (movie != null)
        {
            var movieId = movie.Id;

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

            return Accepted();
        }

        var newMovie = mapper.Map<Entities.Movie>(item);
        appDbContext.Movies.Add(newMovie);
        await appDbContext.SaveChangesAsync();

        return Accepted();
    }

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