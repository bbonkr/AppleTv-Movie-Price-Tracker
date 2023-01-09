using AppleTv.Movie.Price.Tracker.Services;
using AppleTv.Movie.Price.Tracker.Services.Exceptions;
using kr.bbon.AspNetCore.Filters;
using kr.bbon.Core.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AppleTv.Movie.Price.Tracker.App.Infrastructure.Filters;

public class ApiExceptionHandlerWithGitHubIssueFilter : ApiExceptionHandlerFilter
{
    protected override void HandleException(ExceptionContext context)
    {
        base.HandleException(context);

        var gitHubService = context.HttpContext.RequestServices.GetRequiredService<GitHubService>();
        var loggerFactory = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory?.CreateLogger("ExceptionHandlerFilter");
        try
        {
            if (gitHubService != null)
            {
                var method = context.HttpContext.Request.Method;
                var path = context.HttpContext.Request.Path;

                var labels = new string[] { "bug", "help wanted" };

                var cancellationToken = new CancellationTokenSource(10000).Token;

                if (context.Exception is ApiException apiException)
                {
                    // var method = context.ActionDescriptor?.ActionConstraints?.OfType<HttpMethodActionConstraint>().FirstOrDefault()?.HttpMethods.FirstOrDefault() ?? "UNKNOWN";
                    // var controller = context.ActionDescriptor?.RouteValues["controller"] ?? "UNKNOWN-CONTROLLER";
                    // var action = context.ActionDescriptor?.RouteValues["action"] ?? "UNKNOWN-ACTION";

                    if (500 <= (int)apiException.HttpStatusCode)
                    {
                        gitHubService.CreateIssueFromApiExceptionAsync(
                            apiException,
                            $"{method.ToUpper()}: {path}",
                            labels,
                            reopenIfClosedOneExists: true,
                            createNewIssueAlways: false,
                            cancellationToken: cancellationToken)
                            .GetAwaiter()
                            .GetResult();
                    }
                }
                else
                {
                    gitHubService.CreateIssueFromExceptionAsync(
                        context.Exception,
                        $"{method.ToUpper()}: {path}",
                        labels,
                        reopenIfClosedOneExists: true,
                        createNewIssueAlways: false,
                        cancellationToken: cancellationToken)
                        .GetAwaiter()
                        .GetResult();
                }
            }
        }
        catch (GitHubException ex)
        {
            logger?.LogError(@"GitHub Error
error: {message}
document: {url}
", ex.Error.Message, ex.Error.DocumentationUrl);
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Error: {message}", ex.Message);
        }

        context.ExceptionHandled = true;
    }
}

