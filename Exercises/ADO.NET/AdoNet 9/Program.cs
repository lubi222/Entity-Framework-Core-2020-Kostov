using System;
using System.Data;
using System.Text;
using Microsoft.Data.SqlClient;

namespace AdoNet_9
{
    class Program
    {
        private const string ConnectionString = @"Server=.;Database=MinionsDB;Integrated Security=true;";

        static void Main(string[] args)
        {
            using SqlConnection sqlConnection = new SqlConnection(ConnectionString);

            sqlConnection.Open();

            int minionId = int.Parse(Console.ReadLine());

            string result = IncreaseMinionAgeById(sqlConnection, minionId);
            Console.WriteLine(result);

        }

        private static string IncreaseMinionAgeById(SqlConnection sqlConnection, int minionId)
        {
            var sb = new StringBuilder();

            string procName = "usp_GetOlder";
            using SqlCommand increaseAgeCommand = new SqlCommand(procName, sqlConnection);
            increaseAgeCommand.Parameters.AddWithValue("@minionId", minionId);
            increaseAgeCommand.CommandType = CommandType.StoredProcedure;
             

            increaseAgeCommand.ExecuteNonQuery();

            string getMinionInfoQueryText = "Select [Name], Age From Minions Where Id = @minionId";
            using SqlCommand getMinionInfoCommand = new SqlCommand(getMinionInfoQueryText, sqlConnection);
            getMinionInfoCommand.Parameters.AddWithValue("@minionId", minionId);

            using SqlDataReader reader = getMinionInfoCommand.ExecuteReader();

            reader.Read();

            string minionName = reader["Name"]?.ToString();
            string minionAge = reader["Age"]?.ToString();

            sb.AppendLine($"{minionName} - {minionAge} years old");

            return sb.ToString().TrimEnd();
        }
    }
}
