using FluentValidation;
using MediatR;

namespace AppleTv.Movie.Price.Tracker.Domains.Movies.Commands.UntrackMovie;

public class UntrackMovieCommand : IRequest<bool>
{
    public Guid Id { get; set; }
}
