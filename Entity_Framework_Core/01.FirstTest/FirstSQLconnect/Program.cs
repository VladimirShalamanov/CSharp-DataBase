using Microsoft.Data.SqlClient;

string connectionString = "Server=.;Database=SoftUni;Integrated Security=true;TrustServerCertificate=True";

using SqlConnection connection = new SqlConnection(connectionString);

connection.Open();

SqlCommand command = new SqlCommand(
    "SELECT COUNT(*) FROM Employees", connection);

int eCnt = (int) command.ExecuteScalar();

Console.WriteLine("Connected...");
Console.WriteLine($"Employees are: {eCnt}");