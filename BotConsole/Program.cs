using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using TelegramBotSQLite;

var BotToken = "7161910156:AAECxhkkwkF5538utqm9exEtEaA8NrMZnvU";

var botClient = new TelegramBotClient(BotToken);

Database.Initialize();

using var cts = new CancellationTokenSource();

botClient.StartReceiving(
    updateHandler: HandleUpdateAsync,
    pollingErrorHandler: HandlePollingErrorAsync,
    receiverOptions: new ReceiverOptions
    {
        AllowedUpdates = Array.Empty<UpdateType>()
    },
    cancellationToken: cts.Token
);

Console.WriteLine("Бот запущен...");
Console.ReadLine();

cts.Cancel();

async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    if (update.Type == UpdateType.Message && update.Message?.Text != null)
    {
        var message = update.Message;
        string responseText = string.Empty;

        if (message.Text == "/weight")
        {
            var weights = Database.GetWeights(message.From.Username);
            if (weights.Count > 0)
            {
                responseText = "Ваши записи:\n" + string.Join("\n", weights);
            }
            else
            {
                responseText = "Нет записей для вашего пользователя.";
            }
        }
        else if (double.TryParse(message.Text, out double weight))
        {
            if (weight >= 0 && weight <= 300)
            {
                Database.SaveWeight(message.From.Username, weight, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                responseText = "Ваш вес сохранен.";
            }
            else
            {
                responseText = "Вес должен быть в пределах от 0 до 300.";
            }
        }
        else
        {
            responseText = "Ошибка: введите число для сохранения веса.";
        }

        await botClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: responseText,
            cancellationToken: cancellationToken
        );
    }
}

Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
{
    Console.WriteLine($"Произошла ошибка: {exception.Message}");
    return Task.CompletedTask;
}
