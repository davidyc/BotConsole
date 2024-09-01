using BotConsole.DB;
using Telegram.Bot.Types;
using TelegramBotAzureSQL;

namespace BotConsole.Comand
{
    public class Comands
    {
        public static string GetWeights(Message? message)
        {
            var responseText = string.Empty;
            var weights = DatabaseAzure.GetWeights(message.From.Id);
            if (weights.Count > 0)
            {
                responseText = "Ваши записи:\n" + string.Join("\n", weights);
            }
            else
            {
                responseText = "Нет записей для вашего пользователя.";
            }

            return responseText;
        }

        public static string GetAllWeights(Message? message)
        {
            var responseText = string.Empty;
            var weights = DatabaseAzure.GetAllWeights();
            if (weights.Count > 0)
            {
                responseText = "Все записи:\n" + string.Join("\n", weights);
            }
            else
            {
                responseText = "Нет записей для вашего пользователя.";
            }

            return responseText;
        }

        public static string SaveWeight(Message? message)
        {
            var responseText = string.Empty;
           
            if (double.TryParse(message.Text.Replace(',','.'), out double weight))
            {
                var userName = GetUserName(message);
                if (weight >= 0 && weight <= 300)
                {
                    DatabaseAzure.SaveWeight(message.From.Id, userName, weight, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
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

            return responseText;
        }      

        public static string GetUserName(Message? message)
        {
            var userName = string.Empty;
            if (string.IsNullOrEmpty(message.From.Username))
            {
               return $"{message.From.LastName} {message.From.FirstName}";
            }
            return message.From.Username;
        }

    }
}
