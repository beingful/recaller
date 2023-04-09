using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RecallerBot.Models;
using Hangfire;
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
    .AddRequestHandling();

var app = builder.Build();

app.UseHangfireDashboard();
app.MapHangfireDashboard();

app.AddPost(botConfiguration);

app.Run();
