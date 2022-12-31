using AppleTv.Movie.Price.Tracker.Data;
using AppleTv.Movie.Price.Tracker.Domains.Collections.Models;
using AppleTv.Movie.Price.Tracker.Domains.Models;
using AutoMapper;
using kr.bbon.EntityFrameworkCore.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AppleTv.Movie.Price.Tracker.Domains.Collections.Queries.GetCollections;

public class GetCollectionsQueryHandler : IRequestHandler<GetCollectionsQuery, CollectionsPagedModel>
{
    public GetCollectionsQueryHandler(AppDbContext context, IMapper mapper, ILogger<GetCollectionsQueryHandler> logger)
    {
        this.context = context;
        this.mapper = mapper;
        this.logger = logger;
    }

    public async Task<CollectionsPagedModel> Handle(GetCollectionsQuery request, CancellationToken cancellationToken)
    {
        var page = request.page;
        var limit = request.limit;
        var keyword = request.keyword?.Trim();
        var query = context.Collections
            .Include(x => x.Movies)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(x => x.CollectionName.Contains(keyword) || x.CollectionCensoredName.Contains(keyword));
        }

        var result = await query
            .OrderBy(x => x.CollectionName)
            .Select(x => mapper.Map<CollectionListItemModel>(x))
            .AsNoTracking()
            .ToPagedModelAsync<CollectionListItemModel, CollectionsPagedModel>(page, limit, cancellationToken);

        return result;
    }


    private readonly AppDbContext context;
    private readonly IMapper mapper;
    private readonly ILogger logger;
}