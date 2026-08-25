using MechanicShop.Application.Common.Settings;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPresentation(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<AppSettings>(configuration.GetSection("AppSettings"));

            services.AddSingleton(sp => sp.GetRequiredService<IOptions<AppSettings>>().Value);

            return services;
        }

        public static IApplicationBuilder UseCoreMiddlewares(this IApplicationBuilder app, IConfiguration configuration)
        {
            app.UseExceptionHandler();

            app.UseStatusCodePages();

            return app;
        }
    }
}
