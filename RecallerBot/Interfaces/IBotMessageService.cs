namespace RecallerBot.Interfaces;

internal interface IBotMessageService
{
    public Task SendTextMessageAsync(long chatId, string message);
}
