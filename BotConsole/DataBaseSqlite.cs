using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.IO;

namespace TelegramBotSQLite
{
    public static class Database
    {
        private const string DatabaseFile = "TelegramWeights.db";

        public static void Initialize()
        {
            if (!File.Exists(DatabaseFile))
            {
                using var connection = new SqliteConnection($"Data Source={DatabaseFile}");
                connection.Open();

                string createWeightsTable = @"CREATE TABLE Weights (
                                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                        UserName TEXT,
                                        Weight TEXT,
                                        DateRecorded TEXT)";
                using var createWeightsCommand = new SqliteCommand(createWeightsTable, connection);
                createWeightsCommand.ExecuteNonQuery();
            }
        }

        public static void SaveWeight(string userName, double weight, string dateRecorded)
        {
            using var connection = new SqliteConnection($"Data Source={DatabaseFile}");
            connection.Open();

            string insertCommand = "INSERT INTO Weights (UserName, Weight, DateRecorded) VALUES (@UserName, @Weight, @DateRecorded)";
            using var insert = new SqliteCommand(insertCommand, connection);
            insert.Parameters.AddWithValue("@UserName", userName);
            insert.Parameters.AddWithValue("@Weight", weight.ToString());
            insert.Parameters.AddWithValue("@DateRecorded", dateRecorded);
            insert.ExecuteNonQuery();
        }

        public static List<string> GetWeights(string userName)
        {
            var weights = new List<string>();

            using var connection = new SqliteConnection($"Data Source={DatabaseFile}");
            connection.Open();

            string selectCommand = "SELECT Weight, DateRecorded FROM Weights WHERE UserName = @UserName";
            using var select = new SqliteCommand(selectCommand, connection);
            select.Parameters.AddWithValue("@UserName", userName);

            using var reader = select.ExecuteReader();
            while (reader.Read())
            {
                string weight = reader.GetString(0);
                string dateRecorded = reader.GetString(1);
                weights.Add($"Вес: {weight}, Дата записи: {dateRecorded}");
            }

            return weights;
        }
    }
}
