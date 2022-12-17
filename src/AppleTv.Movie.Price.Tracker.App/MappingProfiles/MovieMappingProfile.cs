using AppleTv.Movie.Price.Tracker.Entities;
using AppleTv.Movie.Price.Tracker.Services.Models;

using AutoMapper;

namespace AppleTv.Movie.Price.Tracker.App.MappingProfiles
{
    public class MovieMappingProfile : Profile
    {
        public MovieMappingProfile()
        {
            CreateMap<AppleTv.Movie.Price.Tracker.Entities.Movie, ITunesSearchResultItemModel>()
                .ReverseMap();

        }
    }
}
