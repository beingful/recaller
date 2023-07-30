using RecallerBot.Extensions;
using Flurl.Http.Configuration;
using Flurl.Http;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using RecallerBot.Models.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddAzureWebAppDiagnostics();

builder.Services.SetConfiguration();

var botConfiguration = builder.Configuration
                                .GetSection(nameof(Bot))
                                .Get<Bot>()!;

builder.Services
    .AddAzureAuthentication(builder.Configuration)
    .AddBotConfiguration(botConfiguration)
    .AddWebhook(botConfiguration)
    .AddBotServices()
    .AddScheduling()
    .AddRequestHandling()
    .AddSwagger();

FlurlHttp.Configure(settings =>
{
    settings.JsonSerializer = new NewtonsoftJsonSerializer(new JsonSerializerSettings
    {
        NullValueHandling = NullValueHandling.Ignore,
        ContractResolver = new DefaultContractResolver
        {
            NamingStrategy = new SnakeCaseNamingStrategy()
        },
        MissingMemberHandling = MissingMemberHandling.Ignore
    });
});

var app = builder.Build();

app.UseAuthentication();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Recaller bot API");
    options.RoutePrefix = string.Empty;
});

app.AddHangfireDashboard();

app.AddPost(botConfiguration);

app.Run();
