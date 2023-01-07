using FluentValidation;

namespace AppleTv.Movie.Price.Tracker.Domains.Movies.Commands.TrackMovie;

public class TrackMovieCommandValidator : AbstractValidator<TrackMovieCommand>
{
    public TrackMovieCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage(payload => $"Invalid Id");
    }
}