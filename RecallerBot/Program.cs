using RecallerBot.Models;
using RecallerBot.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddAzureWebAppDiagnostics();

builder.Services.SetConfiguration();

var botConfiguration = builder.Configuration
                                .GetSection(nameof(BotConfiguration))
                                .Get<BotConfiguration>()!;

builder.Services
    .AddBotConfiguration(botConfiguration)
    .AddWebhook(botConfiguration)
    .AddBotServices()
    .AddScheduling()
    .AddRequestHandling()
    .AddSwagger();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Recaller bot API");
    options.RoutePrefix = string.Empty;
});

app.AddHangfireDashboard();

app.AddPost(botConfiguration);

app.Run();
