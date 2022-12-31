using System;
using AppleTv.Movie.Price.Tracker.Domains.Models;
using AppleTv.Movie.Price.Tracker.Domains.Movies.Models;
using AppleTv.Movie.Price.Tracker.Services.Models;
using AutoMapper;
using kr.bbon.Core.Models;

namespace AppleTv.Movie.Price.Tracker.Domains.MappingProfiles;

public class MovieMappingProfile : Profile
{
    public MovieMappingProfile()
    {
        CreateMap<Entities.Movie, ITunesSearchResultItemModel>()
            .ReverseMap();

        CreateMap<Entities.Movie, MovieModel>();

        CreateMap<Entities.Movie, MovieListItemModel>();

    }
}

