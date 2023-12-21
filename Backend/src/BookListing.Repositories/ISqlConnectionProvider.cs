using System.Data.SqlClient;

namespace BookListing.Repositories;
/// <summary>
/// Provider that provides connection to SQL Server database
/// </summary>
public interface ISqlConnectionProvider
{
    SqlConnection GetSqlConnection();
}