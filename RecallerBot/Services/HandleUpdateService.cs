using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using System.Net;
using RecallerBot.Constants;
using RecallerBot.Interfaces;

namespace RecallerBot.Services;

internal sealed class HandleUpdateService
{
    private readonly ChatMessageService _chatMessageService;
    private readonly TimeSheetService _timeSheetService;
    private readonly AlarmClockService _alarmClockService;
    private readonly IBotMessageService _botMessageService;
    private readonly ILogger<HandleUpdateService> _logger;

    public HandleUpdateService(
        ChatMessageService chatMessageService,
        TimeSheetService timeSheetService,
        AlarmClockService alarmClockService,
        IBotMessageService botmessageService,
        ILogger<HandleUpdateService> logger)
    {
        _chatMessageService = chatMessageService;
        _timeSheetService = timeSheetService;
        _alarmClockService = alarmClockService;
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

        if (_chatMessageService.IsCommandRecognized(message))
        {
            if (_chatMessageService.IsChatAllowed(message))
            {
                try
                {
                    HandleMessage(message);

                    await _botMessageService.SendTextMessageAsync(message.Chat.Id, $"{HttpStatusCode.OK}");
                }
                catch (Exception exception)
                {
                    _logger.LogError(ErrorMessages.GeneralError, exception.Message, exception.StackTrace);

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

        Enums.BotCommand command = _chatMessageService.MapToCommand(message);

        if (command == Enums.BotCommand.Start)
        {
            _timeSheetService.StartNotifying(message.Chat.Id);
        }
        else if (command == Enums.BotCommand.Stop)
        {
            _timeSheetService.UnscheduleNotifications(message.Chat.Id);
        }
        else if (command == Enums.BotCommand.AlarmClock)
        {
            _alarmClockService.SetUp();
        }
    }

    private async Task OnUnknownUpdateAsync() =>
        await Task.Run(() =>
        {
            _logger.LogInformation(LogMessages.NotTextMessage);
        });
}
