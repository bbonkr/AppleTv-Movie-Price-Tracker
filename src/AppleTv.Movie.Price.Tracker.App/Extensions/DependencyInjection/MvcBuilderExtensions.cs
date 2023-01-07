using System.Net.Mime;
using System.Text.Json;
using System.Text.Json.Serialization;
using kr.bbon.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace AppleTv.Movie.Price.Tracker.App.Extensions.DependencyInjection;

public static class MvcBuilderExtensions
{
    public static IMvcBuilder ConfigureCustomApiBehaviorOptions(this IMvcBuilder builder)
    {
        builder.ConfigureApiBehaviorOptions(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                PathString path = context.HttpContext.Request.Path;
                string method = context.HttpContext.Request.Method;
                string displayName = context.ActionDescriptor.DisplayName ?? string.Empty;

                var errors = context.ModelState.Values
                    .SelectMany(x => x.Errors)
                    .Select(error => JsonSerializer.Deserialize<ErrorModel>(error.ErrorMessage));

                var responseStatusCode = StatusCodes.Status400BadRequest;
                var responseModel = kr.bbon.AspNetCore.Models.ApiResponseModelFactory.Create(responseStatusCode, "Payload is invalid", errors);

                responseModel.Path = path.ToString();
                responseModel.Method = method;
                responseModel.Instance = displayName;

                context.HttpContext.Response.StatusCode = responseStatusCode;

                return new ObjectResult(responseModel)
                {
                    ContentTypes =
                    {
                    MediaTypeNames.Application.Json,
                    // MediaTypeNames.Application.Xml
                    }
                };
            };
        })
            .ConfigureDefaultJsonOptions()
            .AddXmlSerializerFormatters();

        return builder;
    }

    public static IMvcBuilder ConfigureDefaultJsonOptions(this IMvcBuilder builder)
    {
        builder.AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
            options.JsonSerializerOptions.AllowTrailingCommas = true;
            // options.JsonSerializerOptions.IgnoreReadOnlyFields = true;
            // options.JsonSerializerOptions.IgnoreReadOnlyProperties = true;
            options.JsonSerializerOptions.WriteIndented = true;
            options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals;
            options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            // options.JsonSerializerOptions.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        return builder;
    }

    public static IMvcBuilder ConfigureDefaultXmlOptions(this IMvcBuilder builder)
    {
        builder.AddXmlSerializerFormatters();
        builder.AddXmlDataContractSerializerFormatters();
        // builder.AddXmlOptions();

        return builder;
    }
}