using AppleTv.Movie.Price.Tracker.Data;
using AppleTv.Movie.Price.Tracker.Domains.Models;
using AutoMapper;
using kr.bbon.Core.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AppleTv.Movie.Price.Tracker.Domains.Movies.Queries.GetMovieById;

public class GetMovieByIdQueryHandler : IRequestHandler<GetMovieByIdQuery, MovieModel>
{
    public GetMovieByIdQueryHandler(
        AppDbContext context,
        IMapper mapper,
        ILogger<GetMovieByIdQueryHandler> logger)
    {
        this.context = context;
        this.mapper = mapper;
        this.logger = logger;
    }

    public async Task<MovieModel> Handle(GetMovieByIdQuery request, CancellationToken cancellationToken)
    {
        var result = await context.Movies
            .Include(x => x.Collections)
                .ThenInclude(x => x.Movies)
            .Include(x => x.TrackingLogs)
            .Where(x => x.Id == request.Id)
            .Select(x => mapper.Map<MovieModel>(x))
            .FirstOrDefaultAsync(cancellationToken);

        if (result == null)
        {
            throw new ApiException(System.Net.HttpStatusCode.NotFound, "Movie not found");
        }

        return result;
    }

    private readonly AppDbContext context;
    private readonly IMapper mapper;
    private readonly ILogger logger;
}