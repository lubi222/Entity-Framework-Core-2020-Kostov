using Microsoft.Data.SqlClient;
using System;

namespace AdoNetDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var sqlConnection = new SqlConnection("Server=.;Database=Softuni;Integrated Security=true"))
            {
                sqlConnection.Open();

                //string command = "SELECT COUNT(*) From [Employees]";
                //var sqlCommand = new SqlCommand(command, sqlConnection);

                //object result = sqlCommand.ExecuteScalar();
                //Console.WriteLine(result);

                //sqlConnection.Close();

                var sqlCommand2 = new SqlCommand("SELECT [FirstName], LastName, Salary From [Employees] Where FirstName LIKE 'N%'", sqlConnection);

                using (var reader = sqlCommand2.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string firstName = (string)reader["Firstname"];
                        string lastName = (string)reader["LastName"];
                        decimal salary = (decimal)reader["Salary"];
                        Console.WriteLine(firstName + " " + lastName + " => " + salary);
                    }
                }

                var updateSalaryCommand = new SqlCommand("UPDATE Employees SET Salary = Salary * 1.10", sqlConnection);

                var updatedRows = updateSalaryCommand.ExecuteNonQuery();

                Console.WriteLine($"Salary updated for {updatedRows} employee(s).");

                using (var reader2 = sqlCommand2.ExecuteReader())
                {
                    while (reader2.Read())
                    {
                        string firstName = (string)reader2["Firstname"];
                        string lastName = (string)reader2["LastName"];
                        decimal salary = (decimal)reader2["Salary"];
                        Console.WriteLine(firstName + " " + lastName + " => " + salary);
                    }
                }
            }
        }
    }
}
