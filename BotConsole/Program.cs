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

Console.WriteLine("Bot is up and running...");
Console.ReadLine();

cts.Cancel();

async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    if (update.Type == UpdateType.Message && update.Message?.Text != null)
    {
        var message = update.Message;

        Console.WriteLine($"Received a text message in chat {message.Chat.Id}.");

        Database.SaveMessage(message.Chat.Username ?? "Unknown", message.Text);

        await botClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: "Message received and saved!",
            cancellationToken: cancellationToken
        );
    }
}

Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
{
    Console.WriteLine($"Error occurred: {exception.Message}");
    return Task.CompletedTask;
}
