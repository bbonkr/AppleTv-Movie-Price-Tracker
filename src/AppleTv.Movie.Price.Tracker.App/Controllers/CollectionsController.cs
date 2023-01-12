using AppleTv.Movie.Price.Tracker.Domains.Collections.Models;
using AppleTv.Movie.Price.Tracker.Domains.Collections.Queries.GetCollectionById;
using AppleTv.Movie.Price.Tracker.Domains.Collections.Queries.GetCollections;
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
public class CollectionsController : ApiControllerBase
{

    public CollectionsController(IMediator mediator, ILogger<CollectionsController> logger)
    {
        this.mediator = mediator;
        this.logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<CollectionsPagedModel>> GetCollections([FromQuery] GetCollectionsQuery query)
    {
        var result = await mediator.Send(query);

        return Ok(result);
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<CollectionsPagedModel>> GetCollectionById(long id)
    {
        var query = new GetCollectionByIdQuery(id);
        var result = await mediator.Send(query);

        return Ok(result);
    }


    private readonly IMediator mediator;
    private readonly ILogger logger;
}
