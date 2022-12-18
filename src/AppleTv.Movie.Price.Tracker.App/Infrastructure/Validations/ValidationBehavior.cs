using System;
using FluentValidation;
using kr.bbon.Core.Exceptions;
using kr.bbon.Core.Models;
using MediatR;

namespace AppleTv.Movie.Price.Tracker.App.Infrastructure.Validations;

public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class, IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        this.validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);

        var errorsDictionary = validators
            .Select(x => x.Validate(context))
            .SelectMany(x => x.Errors)
            .Where(x => x != null)
            .GroupBy(
                x => x.PropertyName,
                x => x.ErrorMessage,
                (propertyName, errorMessages) => new
                {
                    Key = propertyName,
                    Values = errorMessages.Distinct().ToArray()
                })
            .ToDictionary(x => x.Key, x => x.Values);

        if (errorsDictionary.Any())
        {
            var httpStatusCode = System.Net.HttpStatusCode.BadRequest;

            var innerErrors = errorsDictionary.Select(x => new ErrorModel(x.Key, Code: x.Key, InnerErrors: x.Value.Select(value => new ErrorModel(value, x.Key)).ToList())).ToList();

            var errors = new ErrorModel("Request body is invalid", httpStatusCode.ToString(), InnerErrors: innerErrors);

            throw new ApiException(httpStatusCode, errors);
        }

        return await next();
    }
}
