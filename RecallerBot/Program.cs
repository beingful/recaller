using RecallerBot.Extensions;
using Flurl.Http.Configuration;
using Flurl.Http;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using RecallerBot.Models.Configuration;
using RecallerBot.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddAzureWebAppDiagnostics();

builder.Services.SetConfiguration();

var botConfiguration = builder.Configuration
                                .GetSection(nameof(Bot))
                                .Get<Bot>()!;

builder.Services
    .AddHttpContextAccessor()
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

app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Recaller bot API");
    options.RoutePrefix = string.Empty;
});

app.AddPost(botConfiguration);

app.MapGet("/get", (GetService getService) =>
{
    string claims = getService.GetClaims();

    return Results.Content(claims);
});

app.AddHangfireDashboard();

app.Run();
