using System;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;
using System.Text;

namespace AdoNet
{
    class Program
    {
        private const string ConnectionString = @"Server=.;Database=MinionsDB;Integrated Security=true;";
        
        static void Main(string[] args)
        {
            using var sqlConnection = new SqlConnection(ConnectionString);

            sqlConnection.Open();

            int villainId = int.Parse(Console.ReadLine());

            string result = GetMinionsInfoAboutVillain(sqlConnection, villainId);

            Console.WriteLine(result);
        }

        private static string GetMinionsInfoAboutVillain(SqlConnection sqlConnection, int villainId)
        {
            var sb = new StringBuilder();

            string villainName = GetVillainName(sqlConnection, villainId);

            if(villainName == null)
            {
                sb.AppendLine($"No villain with Id {villainId} exists in the databse.");
            } else
            {
                sb.AppendLine($"Villain: {villainName}");

                string getMinionsInfoQuery = "Select m.Name, m.Age From Villains as v Left join MinionsVillains as mv on v.Id = mv.VillainId Left join Minions as m on m.Id = mv.MinionId Where v.Name = @villainName Order by m.Name";
                using SqlCommand getMinionsInfoCmd = new SqlCommand(getMinionsInfoQuery, sqlConnection);
                getMinionsInfoCmd.Parameters.AddWithValue("@villainName", villainName);

                using var reader = getMinionsInfoCmd.ExecuteReader();

                    int rowNum = 1;
                    while (reader.Read())
                    {
                        string minionName = reader["Name"]?.ToString();
                        string minionAge = reader["Age"]?.ToString();

                        if (minionName == "" && minionAge == "")
                        {
                            sb.AppendLine("No minions!");
                            break;  
                        }
                        

                        sb.AppendLine($"{rowNum}. {minionName} {minionAge}");

                        rowNum++;
                    }
            }


            return sb.ToString().TrimEnd();
        }

        private static string GetVillainName(SqlConnection sqlConnection, int villainId)
        {
            string getVillainNameQuery = @"Select [Name] From Villains Where Id = @villainId";
            using SqlCommand getVillainNameCmd = new SqlCommand(getVillainNameQuery, sqlConnection);
            getVillainNameCmd.Parameters.AddWithValue("@villainId", villainId);
            string villainName = getVillainNameCmd.ExecuteScalar()?.ToString();

            return villainName;
        }
    }
}
