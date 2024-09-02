using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace TelegramBotAzureSQL
{
    public static class DatabaseAzure
    {
        private const string ConnectionString = "Server=tcp:davidyc-azure.database.windows.net,1433;Initial Catalog=TelegramWeights;Persist Security Info=False;User ID=davidyc-azure;Password=AIGxFz5Y6qQkER2U6OFE;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        public static void Initialize()
        {
            using var connection = new SqlConnection(ConnectionString);
            connection.Open();

            // Создаем таблицу Weights, если она не существует
            string createWeightsTable = @"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Weights' and xtype='U')
                                          CREATE TABLE Weights (
                                          Id INT PRIMARY KEY IDENTITY,
                                          UserId BIGINT NOT NULL,
                                          UserName NVARCHAR(50),
                                          Weight NVARCHAR(10),
                                          DateRecorded DATETIME)";
            using var createWeightsCommand = new SqlCommand(createWeightsTable, connection);
            createWeightsCommand.ExecuteNonQuery();

            // Создаем таблицу Logs, если она не существует
            string createLogsTable = @"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Logs' and xtype='U')
                                       CREATE TABLE Logs (
                                       Id INT PRIMARY KEY IDENTITY,
                                       LogMessage NVARCHAR(MAX),
                                       DateLogged DATETIME)";
            using var createLogsCommand = new SqlCommand(createLogsTable, connection);
            createLogsCommand.ExecuteNonQuery();
        }

        public static void SaveWeight(long userId, string userName, double weight, string dateRecorded)
        {
            using var connection = new SqlConnection(ConnectionString);
            connection.Open();

            string insertCommand = "INSERT INTO Weights (UserId, UserName, Weight, DateRecorded) VALUES (@UserId, @UserName, @Weight, @DateRecorded)";
            using var insert = new SqlCommand(insertCommand, connection);
            insert.Parameters.AddWithValue("@UserId", userId);
            insert.Parameters.AddWithValue("@UserName", userName);
            insert.Parameters.AddWithValue("@Weight", weight.ToString());
            insert.Parameters.AddWithValue("@DateRecorded", DateTime.Parse(dateRecorded));
            insert.ExecuteNonQuery();
        }

        public static List<string> GetWeights(long userId)
        {
            var weights = new List<string>();

            using var connection = new SqlConnection(ConnectionString);
            connection.Open();

            string selectCommand = "SELECT Weight, DateRecorded FROM Weights WHERE UserId = @UserId";
            using var select = new SqlCommand(selectCommand, connection);
            select.Parameters.AddWithValue("@UserId", userId);

            using var reader = select.ExecuteReader();
            while (reader.Read())
            {
                string weight = reader.GetString(0);
                string dateRecorded = reader.GetDateTime(1).ToString("yyyy-MM-dd HH:mm:ss");
                weights.Add($"Вес: {weight}, Дата записи: {dateRecorded}");
            }

            return weights;
        }

        public static List<string> GetAllWeights()
        {
            var weights = new List<string>();

            using var connection = new SqlConnection(ConnectionString);
            connection.Open();

            var selectCommand = "SELECT UserId, UserName, Weight, DateRecorded FROM Weights";
            using var select = new SqlCommand(selectCommand, connection);

            using var reader = select.ExecuteReader();
            while (reader.Read())
            {
                var userId = reader.GetInt64(0);
                var userName = reader.GetString(1);
                var weight = reader.GetString(2);
                var dateRecorded = reader.GetDateTime(3).ToString("yyyy-MM-dd HH:mm:ss");
                weights.Add($"UserId: {userId}, Имя: {userName}, Вес: {weight}, Дата записи: {dateRecorded}");
            }

            return weights;
        }

      
        public static void LogToDatabase(string logMessage)
        {
            using var connection = new SqlConnection(ConnectionString);
            connection.Open();

            string insertCommand = "INSERT INTO Logs (LogMessage, DateLogged) VALUES (@LogMessage, @DateLogged)";
            using var insert = new SqlCommand(insertCommand, connection);
            insert.Parameters.AddWithValue("@LogMessage", logMessage);
            insert.Parameters.AddWithValue("@DateLogged", DateTime.Now);
            insert.ExecuteNonQuery();
        }
    }
}
