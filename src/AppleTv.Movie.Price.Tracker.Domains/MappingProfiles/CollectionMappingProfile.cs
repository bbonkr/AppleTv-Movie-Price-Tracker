using AppleTv.Movie.Price.Tracker.Domains.Models;
using AutoMapper;

namespace AppleTv.Movie.Price.Tracker.Domains.MappingProfiles;

public class CollectionMappingProfile : Profile
{
    public CollectionMappingProfile()
    {
        CreateMap<Entities.Collection, CollectionModel>()
            .ReverseMap();

        CreateMap<Entities.Collection, CollectionListItemModel>();
    }
}

