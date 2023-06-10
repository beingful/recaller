using Hangfire;
using Hangfire.Storage;
using RecallerBot.Activator;
using RecallerBot.Constants;
using RecallerBot.Interfaces;
using RecallerBot.Models;
using RecallerBot.Resolvers;
using RecallerBot.Services;
using Telegram.Bot;

namespace RecallerBot.Extensions;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection SetConfiguration(this IServiceCollection services) =>
        services
            .AddTransient<IConfiguration>(sp =>
            {
                return new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .Build();
            });

    public static IServiceCollection AddBotConfiguration(this IServiceCollection services, BotConfiguration configuration) =>
        services.AddSingleton<BotConfiguration>(configuration);

    public static IServiceCollection AddWebhook(this IServiceCollection services, BotConfiguration configuration)
    {
        services
            .AddHttpClient(WebhookConstants.Name)
            .AddTypedClient<ITelegramBotClient>(httpClient => new TelegramBotClient(configuration.BotToken, httpClient));

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
                options.TimeZoneResolver = new TimeZoneResolver();
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
