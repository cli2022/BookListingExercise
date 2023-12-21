using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace BookListing.Repositories;
/// <summary>
/// Provider that provides connection to SQL Server database
/// </summary>
public class SqlConnectionProvider : ISqlConnectionProvider
{
    private readonly IConfiguration _configuration;
    public SqlConnectionProvider(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public SqlConnection GetSqlConnection()
    {
        return new SqlConnection(_configuration.GetConnectionString("DB"));
    }
}