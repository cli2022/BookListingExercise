using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace BookListing.Repositories.Tests;
public class TestSqlConnectionProvider : ISqlConnectionProvider
{
    public SqlConnection GetSqlConnection()
    {
        return new SqlConnection(new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .Build()
            .GetConnectionString("TestDB"));
    }

    public SqlConnection GetTestMasterDbSqlConnection()
    {
        return new SqlConnection(new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .Build()
            .GetConnectionString("TestMasterDB"));
    }
}