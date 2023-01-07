using FluentValidation;

namespace AppleTv.Movie.Price.Tracker.Domains.Movies.Queries.SearchMovies;

public class SearchMoviesQueryValidator : AbstractValidator<SearchMoviesQuery>
{
    public SearchMoviesQueryValidator()
    {
        RuleFor(x => x.Term).NotNull().NotEmpty().WithMessage(paylaod => $"Search term required");
        RuleFor(x => x.Country).NotNull().NotEmpty().WithMessage(paylaod => $"Country required");
        RuleFor(x => x.Language).NotNull().NotEmpty().WithMessage(paylaod => $"Language required");
    }
}