using System;
using System.Linq;
using System.Text;
using Microsoft.Data.SqlClient;

namespace AdoNet_4
{
    class Program
    {
        private const string ConnectionString = @"Server=.;Database=MinionsDB;Integrated Security=true;";

        static void Main(string[] args)
        {
            using SqlConnection sqlConnection = new SqlConnection(ConnectionString);

            sqlConnection.Open();

            string[] minionsInput = Console.ReadLine()
                .Split(": ", StringSplitOptions.RemoveEmptyEntries)
                .ToArray();

            string[] minionsInfo = minionsInput[1]
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .ToArray();

            string[] villainInfo = minionsInput[2]
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .ToArray();

            string result = AddMinionToDatabase(sqlConnection, minionsInfo, villainInfo);
            Console.WriteLine(result);

        }

        private static string AddMinionToDatabase(SqlConnection sqlConnection, string[] minionsInfo,
            string[] villainInfo)
        {
            var output = new StringBuilder();

            string minionName = minionsInfo[0];
            string minionAge = minionsInfo[1];
            string minionTown = minionsInfo[2];
            //string townCountry = minionsInfo[3];

            string villainName = villainInfo[0];

            string townId = EnsureTownExists(sqlConnection, minionTown, output);
            string villainId = EnsureVillainExists(sqlConnection, villainName, output);

            string insertMinionQueryText =
                @"Insert Into Minions ([Name], Age, TownId) Values (@minionName, @minionAge, @townId)";

            using SqlCommand insertMinionCommand = new SqlCommand(insertMinionQueryText, sqlConnection);
            insertMinionCommand.Parameters.AddRange(new[]
            {
                new SqlParameter("@minionName", minionName),
                new SqlParameter("@minionAge", minionAge),
                new SqlParameter("@townId", townId),
            });

            insertMinionCommand.ExecuteNonQuery();

            string getMinionIdQueryText = @"Select Id From Minions Where [Name] = @minionName";
            using SqlCommand getMinionIdCommand = new SqlCommand(getMinionIdQueryText, sqlConnection);

            getMinionIdCommand.Parameters.AddWithValue("@minionName", minionName);

            string minionId = getMinionIdCommand.ExecuteScalar().ToString();

            // connecting the minion to its villain - insert both ids in the matching table

            string insertIntoMappingTableQueryText =
                @"Insert into MinionsVillains(MinionId, VIllainId) Values (@minionId, @villainId)";
            using SqlCommand insertIntoMappingTableCommand =
                new SqlCommand(insertIntoMappingTableQueryText, sqlConnection);
            insertIntoMappingTableCommand.Parameters.AddRange(new[]
            {
                new SqlParameter("@minionId", minionId),
                new SqlParameter("@villainId", villainId)
            });

            insertIntoMappingTableCommand.ExecuteNonQuery();
            output.AppendLine($"Successfully added {minionName} to be minion of {villainName}");
            
            return output.ToString().TrimEnd();

        }

        private static string EnsureTownExists(SqlConnection sqlConnection, string minionTown, StringBuilder output)
        {
            string getTownIdQueryText = @"Select Id From Towns Where Name = @townName";

            using SqlCommand getTownIdCmd = new SqlCommand(getTownIdQueryText, sqlConnection);
            getTownIdCmd.Parameters.AddWithValue("@townName", minionTown);

            // May be missing
            string townId = getTownIdCmd.ExecuteScalar()?.ToString();

            if (townId == null)
            {
                string insertTownQueryText = @"Insert into towns([Name], CountryCode) Values (@townName, 1)";
                using SqlCommand insertTownCmd = new SqlCommand(insertTownQueryText, sqlConnection);

                insertTownCmd.Parameters.AddWithValue("@townName", minionTown);
                insertTownCmd.ExecuteNonQuery();

                townId = getTownIdCmd.ExecuteScalar().ToString();

                output.AppendLine($"Town {minionTown} was added to the database.");
            }

            return townId;
        }

        private static string EnsureVillainExists(SqlConnection sqlConnection, string villainName, StringBuilder output)
        {
            string getVillainIdQueryText = @"Select Id From Villains Where [Name] = @name";
            using SqlCommand getVillainIdCmd = new SqlCommand(getVillainIdQueryText, sqlConnection);

            getVillainIdCmd.Parameters.AddWithValue("@name", villainName);

            string villainId = getVillainIdCmd.ExecuteScalar()?.ToString();

            if (villainId == null)
            {
                // get the Id which corresponds to the factor of 'evil' so we can use it as a default value when inserting a new record to the villains 

                string getFactorIdQueryText = @"Select Id From EvilnessFactors Where [Name] = 'Evil'";
                using SqlCommand getFactorIdCmd = new SqlCommand(getFactorIdQueryText, sqlConnection);

                string factorId = getFactorIdCmd.ExecuteScalar()?.ToString();

                string insertVillainQueryText =
                    @"Insert into Villains ([Name], EvilnessFactorId) Values (@villainName, @factorid)";

                using SqlCommand insertVillainCmd = new SqlCommand(insertVillainQueryText, sqlConnection);
                insertVillainCmd.Parameters.AddWithValue("@villainName", villainName);
                insertVillainCmd.Parameters.AddWithValue("@factorid", factorId);

                insertVillainCmd.ExecuteNonQuery();

                villainId = getVillainIdCmd.ExecuteScalar().ToString();

                // add to the output messages
                output.AppendLine($"Villain {villainName} was added to the database");
            }

            return villainId;
        }
    }
}
