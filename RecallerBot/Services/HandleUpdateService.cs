using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using System.Net;
using RecallerBot.Constants;

namespace RecallerBot.Services;

internal sealed class HandleUpdateService
{
    private readonly MessageService _messageService;
    private readonly ScheduleService _scheduleService;
    private readonly ITelegramBotClient _botClient;
    private readonly ILogger<HandleUpdateService> _logger;

    public HandleUpdateService(
        MessageService messageService,
        ScheduleService scheduleService,
        ITelegramBotClient botClient,
        ILogger<HandleUpdateService> logger)
    {
        _messageService = messageService;
        _scheduleService = scheduleService;
        _botClient = botClient;
        _logger = logger;
    }

    public async Task StartAsync(Update update) =>
        await (update.Type switch
        {
            UpdateType.Message => OnMessageReceivedAsync(update.Message!),
            _ => OnUnknownUpdateAsync()
        });

    private async Task OnMessageReceivedAsync(Message message)
    {
        _logger.LogInformation(LogMessages.MessageReceived, message.Text, message.Chat.Id);

        if (_messageService.IsCommandRecognized(message))
        {
            if (_messageService.IsChatAllowed(message))
            {
                try
                {
                    HandleMessage(message);

                    await _botClient.SendTextMessageAsync(message.Chat.Id, $"{HttpStatusCode.OK}");
                }
                catch (Exception exception)
                {
                    _logger.LogError(LogMessages.Error, exception.Message, exception.StackTrace);

                    await _botClient.SendTextMessageAsync(message.Chat.Id, $"{HttpStatusCode.NotImplemented}");
                }
            }
            else
            {
                _logger.LogInformation(LogMessages.ChatNotAllowed);

                await _botClient.SendTextMessageAsync(message.Chat.Id, $"{HttpStatusCode.Forbidden}");
            }
        }
        else
        {
            _logger.LogInformation(LogMessages.CommandNotRecognized);
        }
    }

    private void HandleMessage(Message message)
    {
        _logger.LogInformation(LogMessages.StartHandleMessage);

        Enums.BotCommand command = _messageService.MapToCommand(message);

        if (command == Enums.BotCommand.Start)
        {
            _scheduleService.Schedule(async (scheduledMessage) =>
            {
                await _botClient.SendTextMessageAsync(message.Chat.Id, scheduledMessage);
            });
        }
        else if (command == Enums.BotCommand.Stop)
        {
            _scheduleService.Unschedule();
        }
        else if (command == Enums.BotCommand.Test)
        {
            _scheduleService.ScheduleTest(async (scheduledMessage) =>
            {
                await _botClient.SendTextMessageAsync(message.Chat.Id, scheduledMessage);
            });
        }
    }

    private async Task OnUnknownUpdateAsync() =>
        await Task.Run(() =>
        {
            _logger.LogInformation(LogMessages.NotTextMessage);
        });
}
