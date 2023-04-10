using Hangfire;
using Hangfire.Storage;
using RecallerBot.Constants;
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

    public static IServiceCollection AddScheduling(this IServiceCollection services) =>
        services
            .AddHangfire(configuration =>
            {
                configuration
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
            .AddScoped<ScheduleService>();

    public static IServiceCollection AddWebhook(this IServiceCollection services, BotConfiguration configuration)
    {
        services
            .AddHttpClient(WebhookConstants.Name)
            .AddTypedClient<ITelegramBotClient>(httpClient => new TelegramBotClient(configuration.BotToken, httpClient));

        return services;
    }

    public static IServiceCollection AddRequestHandling(this IServiceCollection services) =>
        services
            .AddScoped<HandleUpdateService>()
            .AddScoped<MessageService>()
            .AddHostedService<WebhookService>();

    public static IServiceCollection AddSwagger(this IServiceCollection services) =>
        services
            .AddEndpointsApiExplorer()
            .AddSwaggerGen();
}
