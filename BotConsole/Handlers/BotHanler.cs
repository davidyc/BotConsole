using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot;
using BotConsole.Comand;
using Newtonsoft.Json;
using TelegramBotAzureSQL;

namespace BotConsole.Handlers
{
    public class BotHanler
    {      
        public async static Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            DatabaseAzure.LogToDatabase( JsonConvert.SerializeObject(update));
            if (update.Type == UpdateType.Message && update.Message?.Text != null)
            {
                var message = update.Message;
                var responseText = string.Empty;

                switch (message.Text)
                {
                    case "/weight":
                        responseText = Comands.GetWeights(message);
                        break;

                    case "/admin":
                        responseText = Comands.GetAllWeights(message);
                        break;

                    default:
                      responseText = Comands.SaveWeight(message);    
                        break;
                }

                DatabaseAzure.LogToDatabase(JsonConvert.SerializeObject(responseText));
                await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: responseText,
                    cancellationToken: cancellationToken
                );
                DatabaseAzure.LogToDatabase("/nОтправлено/n!");
            }
        }

        public static Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Произошла ошибка: {exception.Message}");
            DatabaseAzure.LogToDatabase($"Произошла ошибка: {exception.Message}");
            return Task.CompletedTask;
        }

      

    }
}
