using AppleTv.Movie.Price.Tracker.Data;
using AppleTv.Movie.Price.Tracker.Domains.Models;
using AutoMapper;
using kr.bbon.Core.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AppleTv.Movie.Price.Tracker.Domains.Collections.Queries.GetCollectionById;

public class GetCollectionByIdQueryHandler : IRequestHandler<GetCollectionByIdQuery, CollectionModel>
{
    public GetCollectionByIdQueryHandler(AppDbContext context, IMapper mapper, ILogger<GetCollectionByIdQueryHandler> logger)
    {
        this.context = context;
        this.mapper = mapper;
        this.logger = logger;
    }

    public async Task<CollectionModel> Handle(GetCollectionByIdQuery request, CancellationToken cancellationToken)
    {
        var query = context.Collections
            .Include(x => x.Movies)
            .Where(x => x.CollectionId == request.Id);

        var model = await query
            .OrderBy(x => x.Id)
            .Select(x => mapper.Map<CollectionModel>(x))
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);

        if (model == null)
        {
            throw new ApiException(System.Net.HttpStatusCode.NotFound, "Collection not found");
        }

        return model;
    }


    private readonly AppDbContext context;
    private readonly IMapper mapper;
    private readonly ILogger logger;
}