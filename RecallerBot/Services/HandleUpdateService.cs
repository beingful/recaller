using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using System.Net;
using RecallerBot.Constants;
using RecallerBot.Interfaces;

namespace RecallerBot.Services;

internal sealed class HandleUpdateService
{
    private readonly MessageValidationService _messageService;
    private readonly ScheduleService _scheduleService;
    private readonly IBotMessageService _botMessageService;
    private readonly ILogger<HandleUpdateService> _logger;

    public HandleUpdateService(
        MessageValidationService messageService,
        ScheduleService scheduleService,
        IBotMessageService botmessageService,
        ILogger<HandleUpdateService> logger)
    {
        _messageService = messageService;
        _scheduleService = scheduleService;
        _botMessageService = botmessageService;
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

                    await _botMessageService.SendTextMessageAsync(message.Chat.Id, $"{HttpStatusCode.OK}");
                }
                catch (Exception exception)
                {
                    _logger.LogError(LogMessages.Error, exception.Message, exception.StackTrace);

                    await _botMessageService.SendTextMessageAsync(message.Chat.Id, $"{HttpStatusCode.NotImplemented}");
                }
            }
            else
            {
                _logger.LogInformation(LogMessages.ChatNotAllowed);

                await _botMessageService.SendTextMessageAsync(message.Chat.Id, $"{HttpStatusCode.Forbidden}");
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
            _scheduleService.Schedule(message.Chat.Id);
        }
        else if (command == Enums.BotCommand.Stop)
        {
            _scheduleService.Unschedule();
        }
        else if (command == Enums.BotCommand.Test)
        {
            _scheduleService.ScheduleTest(message.Chat.Id);
        }
    }

    private async Task OnUnknownUpdateAsync() =>
        await Task.Run(() =>
        {
            _logger.LogInformation(LogMessages.NotTextMessage);
        });
}
