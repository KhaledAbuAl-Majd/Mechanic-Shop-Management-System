using System.Text;
using MechanicShop.Application.Common.Constants;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Common.Settings;
using MechanicShop.Application.Features.Billing.Interfaces;
using MechanicShop.Application.Features.Identity.Interfaces;
using MechanicShop.Domain.Identity.Enums;
using MechanicShop.Infrastructure.BackgroundJobs;
using MechanicShop.Infrastructure.Identity;
using MechanicShop.Infrastructure.Identity.Polices;
using MechanicShop.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using QuestPDF.Infrastructure;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuraion)
    {
        services.AddSingleton(TimeProvider.System);

        var jwtSettings = configuraion.GetSection(JwtSettings.SectionName).Get<JwtSettings>();

        ArgumentNullException.ThrowIfNull(jwtSettings);

        services.TryAddSingleton(jwtSettings);


        QuestPDF.Settings.License = LicenseType.Community;

        services.AddBackgroundServices();
        services.AddData();
        services.AddIdentity(jwtSettings);
        services.AddServices();


        return services;
    }

    private static IServiceCollection AddData(this IServiceCollection services)
    {


        return services;
    }

    private static IServiceCollection AddBackgroundServices(this IServiceCollection services)
    {
        services.AddHostedService<OverdueBookingCleanupService>();

        return services;
    }

    private static IServiceCollection AddIdentity(this IServiceCollection services, JwtSettings jwtSettings)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret))
            };
        });

        services.AddScoped<IAuthorizationHandler, LaborAssignedHandler>();

        services.AddAuthorizationBuilder()
            .AddPolicy(AuthorizationPolicies.ManagerOnly, policy => policy.RequireRole(nameof(Role.Manager)))
            .AddPolicy(AuthorizationPolicies.SelfScopedWorkOrderAccess, policy =>
            policy.Requirements.Add(new LaborAssignedRequirement()));


        services.TryAddScoped<IIdentityService, IdentityService>();
        services.TryAddScoped<ITokenProvider, TokenProvider>();

        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {

        services.TryAddSingleton<IInvoicePdfGenerator, InvoicePdfGenerator>();
        services.TryAddSingleton<INotificationService, NotificationService>();

        return services;
    }
}
