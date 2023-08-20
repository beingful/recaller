using Hangfire;
using Hangfire.Storage;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using RecallerBot.Activator;
using RecallerBot.Constants;
using RecallerBot.Interfaces;
using RecallerBot.Models.Configuration;
using RecallerBot.Services;
using System.Security.Claims;
using Telegram.Bot;
using Microsoft.Identity.Web;

namespace RecallerBot.Extensions;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAzureAuthentication(this IServiceCollection services, ConfigurationManager configuration)
    {
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                Authentication authentication = configuration
                                            .GetSection(nameof(Authentication))
                                            .Get<Authentication>()!;

                options.Authority = authentication.Authority;
                options.TokenValidationParameters.ValidateAudience = false;
            })
            .AddMicrosoftIdentityWebApi(configuration)
            .EnableTokenAcquisitionToCallDownstreamApi()
            .AddMicrosoftGraph()
            .AddInMemoryTokenCaches();
        //services
        //    .AddAuthentication()
        //    .AddOpenIdConnect("AzureOpenId", "Azure Active Directory OpenId", options =>
        //    {
        //        Authentication authentication = configuration
        //                                    .GetSection(nameof(Authentication))
        //                                    .Get<Authentication>()!;

        //        options.Authority = authentication.Authority;
        //        options.ClientId = configuration["AzureAd:ClientId"];
        //        options.ClientSecret = configuration["AzureAd:ClientSecret"];
        //        options.RequireHttpsMetadata = false;
        //        options.GetClaimsFromUserInfoEndpoint = true;
        //        options.ResponseType = OpenIdConnectResponseType.Code;
        //        options.SaveTokens = true;
        //        //options.Scope.Add("email");
        //        //options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "sub");
        //    });

        //services
        //    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        //    .AddMicrosoftIdentityWebApp(options =>
        //    {
        //        configuration.Bind("AzureAd", options);

        //        options.TokenValidationParameters.NameClaimType = "name";
        //    },
        //    options => configuration.Bind("AzureAd", options));

        //configuration.GetSection("AzureAd");
        //services
        //    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        //    .AddJwtBearer(options =>
        //    {
        //        Authentication authentication = configuration
        //                                    .GetSection(nameof(Authentication))
        //                                    .Get<Authentication>()!;

        //        options.Authority = authentication.Authority;
        //        options.Audience = authentication.Audience;
        //    });

        return services;
    }

    public static IServiceCollection SetConfiguration(this IServiceCollection services) =>
        services
            .AddTransient<IConfiguration>(sp =>
            {
                return new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .Build();
            });

    public static IServiceCollection AddBotConfiguration(this IServiceCollection services, Bot bot) =>
        services.AddSingleton<Bot>(bot);

    public static IServiceCollection AddWebhook(this IServiceCollection services, Bot bot)
    {
        services
            .AddHttpClient(WebhookConstants.Name)
            .AddTypedClient<ITelegramBotClient>(httpClient => new TelegramBotClient(bot.Token, httpClient));

        return services;
    }

    public static IServiceCollection AddBotServices(this IServiceCollection services) =>
        services
            .AddSingleton<IBotEndpointService, TelegramBotService>()
            .AddScoped<IBotMessageService, TelegramBotService>()
            .AddScoped<ChatMessageService>();

    public static IServiceCollection AddScheduling(this IServiceCollection services) =>
        services
            .AddScoped<NotificationService>()
            .AddScoped<BotRequestService>()
            .AddHangfire(configuration =>
            {
                configuration
                    .UseActivator(new HangfireActivator(services.BuildServiceProvider()))
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings()
                    .UseInMemoryStorage();
            })
            .AddHangfireServer(options =>
            {
                options.WorkerCount = 1;
            })
            .AddScoped<IStorageConnection>(sp => JobStorage.Current.GetConnection())
            .AddScoped<ScheduleService>()
            .AddScoped<TimeSheetService>()
            .AddScoped<AlarmClockService>();

    public static IServiceCollection AddRequestHandling(this IServiceCollection services) =>
        services
            .AddScoped<HandleUpdateService>()
            .AddHostedService<WebhookService>();

    public static IServiceCollection AddSwagger(this IServiceCollection services) =>
        services
            .AddEndpointsApiExplorer()
            .AddSwaggerGen();
}
