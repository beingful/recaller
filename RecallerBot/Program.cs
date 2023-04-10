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
    .AddScheduling()
    .AddWebhook(botConfiguration)
    .AddRequestHandling()
    .AddSwagger();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.AddHangfireDashboard();

app.AddPost(botConfiguration);

app.Run();
