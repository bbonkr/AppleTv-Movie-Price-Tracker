using AppleTv.Movie.Price.Tracker.Domains.Models;
using AutoMapper;

namespace AppleTv.Movie.Price.Tracker.Domains.MappingProfiles;

public class MoviePriceMappingProfile : Profile
{
    public MoviePriceMappingProfile()
    {
        CreateMap<Entities.MoviePrice, MoviePriceModel>()
            .ReverseMap();
    }
}

