using Hangfire;
using Hangfire.Storage;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using RecallerBot.Activator;
using RecallerBot.Constants;
using RecallerBot.Interfaces;
using RecallerBot.Models.Configuration;
using RecallerBot.Services;
using Telegram.Bot;

namespace RecallerBot.Extensions;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAzureAuthentication(this IServiceCollection services, ConfigurationManager configuration)
    {
        services
            .AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApp(options =>
            {
                AzureAd azureAd = configuration
                    .GetSection(nameof(AzureAd))
                    .Get<AzureAd>()!;

                options.Instance = azureAd.Instance;
                options.Domain = azureAd.Domain;
                options.TenantId = azureAd.TenantId;
                options.Authority = azureAd.Authority;
                options.ClientId = azureAd.ClientId;
                options.ClientSecret = azureAd.ClientSecret;
                options.CallbackPath = azureAd.CallbackPath;

                options.ResponseType = OpenIdConnectResponseType.Code;

                options.Scope.Add(azureAd.Scope);
            });
            //{
            //    options.Scope
            //});
            //.EnableTokenAcquisitionToCallDownstreamApi()
            //.AddMicrosoftGraph()
            //.AddInMemoryTokenCaches();

        return services.AddAuthorization();
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
            .AddScoped<ChatMessageService>()
            .AddScoped<GetService>();

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
