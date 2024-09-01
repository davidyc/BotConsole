using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using BotConsole.Handlers;

using TelegramBotAzureSQL;

var BotToken = "7161910156:AAECxhkkwkF5538utqm9exEtEaA8NrMZnvU";

var botClient = new TelegramBotClient(BotToken);
await botClient.DeleteWebhookAsync();

DatabaseAzure.Initialize();

using var cts = new CancellationTokenSource();

botClient.StartReceiving(
    updateHandler: BotHanler.HandleUpdateAsync,
    pollingErrorHandler: BotHanler.HandlePollingErrorAsync,
    receiverOptions: new ReceiverOptions
    {
        AllowedUpdates = Array.Empty<UpdateType>()
    },
    cancellationToken: cts.Token
);

Console.WriteLine("Бот запущен...");


while (true){};
