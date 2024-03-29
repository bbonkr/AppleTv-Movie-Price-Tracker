﻿using System;
using AppleTv.Movie.Price.Tracker.Domains.Models;
using AppleTv.Movie.Price.Tracker.Services.Models;
using AutoMapper;

namespace AppleTv.Movie.Price.Tracker.Domains.MappingProfiles;

public class MovieMappingProfile : Profile
{
    public MovieMappingProfile()
    {
        CreateMap<Entities.Movie, ITunesSearchResultItemModel>()
            .ReverseMap();

        CreateMap<Entities.Movie, MovieModel>();

        CreateMap<Entities.Movie, MovieListItemModel>();

        CreateMap<Entities.Movie, MovieSimpleModel>();
    }
}

