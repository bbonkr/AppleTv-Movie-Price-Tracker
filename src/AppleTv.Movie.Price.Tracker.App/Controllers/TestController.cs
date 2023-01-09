using System.Net;
using kr.bbon.AspNetCore;
using kr.bbon.AspNetCore.Mvc;
using kr.bbon.Core.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppleTv.Movie.Price.Tracker.App.Controllers;

[Authorize]
[ApiController]
[Area(DefaultValues.AreaName)]
[Route(DefaultValues.RouteTemplate)]
[ApiVersion(DefaultValues.ApiVersion)]
[Produces(Constants.RESPONSE_MEDIA_TYPE)]
public class TestController : ApiControllerBase
{
    [HttpGet("genericexception")]
    public IActionResult ThrowsException()
    {
        throw new Exception("Test ;)");
    }

    [HttpGet("apiexception")]
    public IActionResult ThrowApiException()
    {
        throw new ApiException(HttpStatusCode.InternalServerError, "Test ;)");
    }
}