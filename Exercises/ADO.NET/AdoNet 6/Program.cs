using System;
using System.Text;
using Microsoft.Data.SqlClient;

namespace AdoNet_6
{
    class Program
    {
        private const string ConnectionString = @"Server=.;Database=MinionsDB;Integrated Security=true;";
        static void Main(string[] args)
        {
            using SqlConnection sqlConnection = new SqlConnection(ConnectionString);

            sqlConnection.Open();

            int villainId = int.Parse(Console.ReadLine());

            string result = RemoveVillainById(sqlConnection, villainId);

            Console.WriteLine(result);
        }

        private static string RemoveVillainById(SqlConnection sqlConnection, int villainId)
        {
            var sb = new StringBuilder();

            using SqlTransaction sqlTransaction = sqlConnection.BeginTransaction();

            // get the villain 

            string getVillainNameQueryText = @"Select [Name] From Villains Where Id = @villainId";
            using SqlCommand getVillainCommand = new SqlCommand(getVillainNameQueryText, sqlConnection);
            getVillainCommand.Parameters.AddWithValue("@villainId", villainId);
            getVillainCommand.Transaction = sqlTransaction;

            string villainName = getVillainCommand.ExecuteScalar()?.ToString();

            if (villainName == null)
            {
                sb.AppendLine($"No such villain was found");
            }
            else
            {
                try
                {
                    // here we try to delete 
                    string releaseMinionsQueryText = @"Delete From MinionsVillains Where VillainId = @villainId";
                    using SqlCommand releaseMinionsCommand = new SqlCommand(releaseMinionsQueryText, sqlConnection);
                    releaseMinionsCommand.Parameters.AddWithValue("@villainId", villainId);
                    
                    releaseMinionsCommand.Transaction = sqlTransaction;
                    // returns how many rows the execution of the command has affected 
                    int releasedMinionsCount = releaseMinionsCommand.ExecuteNonQuery();

                    string deleteVillainQueryText = @"Delete From Villains Where Id = @villainId";
                    using SqlCommand deleteVillainCommand = new SqlCommand(deleteVillainQueryText, sqlConnection);
                    deleteVillainCommand.Parameters.AddWithValue("@villainId", villainId);
                    deleteVillainCommand.Transaction = sqlTransaction;
                    deleteVillainCommand.ExecuteNonQuery();

                    sqlTransaction.Commit();
                    sb
                        .AppendLine($"{villainName} was deleted.")
                        .AppendLine($"{releasedMinionsCount} minions were released");
                }
                catch (Exception e)
                {
                    sb.AppendLine(e.Message);
                    try
                    {
                        sqlTransaction.Rollback();
                    }
                    catch (Exception rollbackEx)
                    {
                        sb.AppendLine(rollbackEx.Message);

                    }
                }

            }

            return sb.ToString().TrimEnd();
        }
    }
}
