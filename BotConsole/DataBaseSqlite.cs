using Microsoft.Data.Sqlite;
using System.IO;

namespace TelegramBotSQLite
{
    public static class Database
    {
        private const string DatabaseFile = "TelegramMessages.db";

        public static void Initialize()
        {
            if (!File.Exists(DatabaseFile))
            {
                using var connection = new SqliteConnection($"Data Source={DatabaseFile}");
                connection.Open();

                string tableCommand = @"CREATE TABLE Messages (
                                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                        UserName TEXT,
                                        MessageText TEXT,
                                        DateReceived TEXT)";
                using var createTable = new SqliteCommand(tableCommand, connection);
                createTable.ExecuteNonQuery();
            }
        }

        public static void SaveMessage(string userName, string messageText)
        {
            using var connection = new SqliteConnection($"Data Source={DatabaseFile}");
            connection.Open();

            string insertCommand = "INSERT INTO Messages (UserName, MessageText, DateReceived) VALUES (@UserName, @MessageText, @DateReceived)";
            using var insert = new SqliteCommand(insertCommand, connection);
            insert.Parameters.AddWithValue("@UserName", userName);
            insert.Parameters.AddWithValue("@MessageText", messageText);
            insert.Parameters.AddWithValue("@DateReceived", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            insert.ExecuteNonQuery();
        }
    }
}
