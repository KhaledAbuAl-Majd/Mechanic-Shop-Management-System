using System.Reflection;
using System.Text.Json.Serialization;
using Asp.Versioning;
using FluentValidation;
using MechanicShop.Api.Infrastructure;
using MechanicShop.Api.OpenApi.Transformers;
using MechanicShop.Api.Services;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Common.Settings;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPresentation(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<AppSettings>(configuration.GetSection("AppSettings"));

            services.AddSingleton(sp => sp.GetRequiredService<IOptions<AppSettings>>().Value);

            services.AddCustomProblemDetails()
                    .AddCustomApiVersioning()
                    .AddApiDocumentation()
                    .AddExceptionHandling()
                    .AddControllerWithJsonConfiguration()
                    .AddCurrentUserService()
                    .AddAppOutputCaching();

            return services;
        }

        private static IServiceCollection AddAppOutputCaching(this IServiceCollection services)
        {
            services.AddOutputCache(options =>
            {
                options.SizeLimit = 100 * 1024 * 1024;//100 mb

                options.AddBasePolicy(policy =>
                {
                    policy.Expire(TimeSpan.FromSeconds(60));
                });

                options.UseCaseSensitivePaths = false;
            });

            return services;
        }

        private static IServiceCollection AddCustomProblemDetails(this IServiceCollection services)
        {
            services.AddProblemDetails(options =>
            {
                options.CustomizeProblemDetails = context =>
                {
                    context.ProblemDetails.Instance = $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
                    context.ProblemDetails.Extensions.Add("requestId", context.HttpContext.TraceIdentifier);
                };
            });

            return services;
        }

        private static IServiceCollection AddCustomApiVersioning(this IServiceCollection services)
        {
            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            })
            .AddMvc()
            .AddApiExplorer(options=>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            return services;
        }

        private static IServiceCollection AddApiDocumentation(this IServiceCollection services)
        {
            string[] versions = ["v1"];

            foreach(var version in versions)
            {
                services.AddOpenApi(version, options =>
                {
                    options.AddDocumentTransformer<VersionInfoTransformer>();

                    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();

                    options.AddOperationTransformer<BearerSecurityOperationTransformer>();
                });
            }

            return services;
        }

        private static IServiceCollection AddExceptionHandling(this IServiceCollection services)
        {
            services.AddExceptionHandler<GlobalExceptionHandler>();

            return services;
        }

        private static IServiceCollection AddControllerWithJsonConfiguration(this IServiceCollection services)
        {
            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });


            return services;
        }

        private static IServiceCollection AddCurrentUserService(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.TryAddScoped<IUser, CurrentUser>();

            return services;
        }

        public static IApplicationBuilder UseCoreMiddlewares(this IApplicationBuilder app, IConfiguration configuration)
        {
            app.UseExceptionHandler();

            app.UseStatusCodePages();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseOutputCache();

            return app;
        }
    }
}
