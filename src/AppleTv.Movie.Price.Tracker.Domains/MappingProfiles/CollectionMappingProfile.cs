using AppleTv.Movie.Price.Tracker.Domains.Collections.Models;
using AppleTv.Movie.Price.Tracker.Domains.Models;
using AutoMapper;
using kr.bbon.Core.Models;

namespace AppleTv.Movie.Price.Tracker.Domains.MappingProfiles;

public class CollectionMappingProfile : Profile
{
    public CollectionMappingProfile()
    {
        CreateMap<Entities.Collection, CollectionModel>();

        CreateMap<Entities.Collection, CollectionListItemModel>();
    }
}

