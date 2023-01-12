using kr.bbon.AspNetCore.Filters;
using kr.bbon.Core.Exceptions;
using kr.bbon.Services.GitHub;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AppleTv.Movie.Price.Tracker.App.Infrastructure.Filters;

public class ApiExceptionHandlerWithGitHubIssueFilter : ApiExceptionHandlerFilter
{
    protected override void HandleException(ExceptionContext context)
    {
        base.HandleException(context);
        var scope = context.HttpContext.RequestServices.CreateScope();
        var loggerFactory = scope?.ServiceProvider.GetRequiredService<ILoggerFactory>();
        var gitHubService = scope?.ServiceProvider.GetRequiredService<GitHubService>();

        var logger = loggerFactory?.CreateLogger("ExceptionHandlerFilter");
        try
        {
            if (gitHubService != null)
            {
                var method = context.HttpContext.Request.Method;
                var path = context.HttpContext.Request.Path;

                var cancellationToken = new CancellationTokenSource(5000).Token;

                Action? gitHubServiceActioin = null;
                if (context.Exception is ApiException apiException)
                {
                    if (500 <= (int)apiException.HttpStatusCode)
                    {
                        gitHubServiceActioin = new Action(async () =>
                            await gitHubService.CreateIssueFromApiExceptionAsync(
                                apiException,
                                $"{method.ToUpper()}: {path}",
                                Constants.ISSUE_LABELS,
                                cancellationToken: cancellationToken));
                    }
                }
                else
                {
                    gitHubServiceActioin = new Action(async () =>
                        await gitHubService.CreateIssueFromExceptionAsync(
                            context.Exception,
                            $"{method.ToUpper()}: {path}",
                            Constants.ISSUE_LABELS,
                            cancellationToken: cancellationToken));
                }

                gitHubServiceActioin?.Invoke();
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

