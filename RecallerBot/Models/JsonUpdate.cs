using Newtonsoft.Json;
using System.Reflection;
using Telegram.Bot.Types;

namespace RecallerBot.Models;

internal sealed class JsonUpdate : Update
{
    public static async ValueTask<JsonUpdate?> BindAsync(HttpContext context, ParameterInfo parameter)
    {
        using var streamReader = new StreamReader(context.Request.Body);

        var updateJsonString = await streamReader.ReadToEndAsync();

        return JsonConvert.DeserializeObject<JsonUpdate>(updateJsonString);
    }
}
