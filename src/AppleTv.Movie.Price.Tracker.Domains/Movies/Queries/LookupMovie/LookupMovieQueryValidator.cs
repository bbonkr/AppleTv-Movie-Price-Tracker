using FluentValidation;

namespace AppleTv.Movie.Price.Tracker.Domains.Movies.Queries.LookupMovie;

public class LookupMovieQueryValidator : AbstractValidator<LookupMovieQuery>
{
    public LookupMovieQueryValidator()
    {
        RuleFor(x => x.TrackId).GreaterThan(0).WithMessage(payload => $"Invalid trackId");
    }
}