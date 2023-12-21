using System.Data.SqlClient;
using Dapper;
using Xunit;

[assembly: CollectionBehavior(CollectionBehavior.CollectionPerAssembly, MaxParallelThreads = 1, DisableTestParallelization = true)]

namespace BookListing.Repositories.Tests;
public static class TestDbHelper
{
    public static SqlConnection GetTestDbSqlConnection()
    {
        return new TestSqlConnectionProvider().GetSqlConnection();
    }

    public static SqlConnection GetTestMasterDbSqlConnection()
    {
        return new TestSqlConnectionProvider().GetTestMasterDbSqlConnection();
    }

    public static async Task CreateTestDbIfNotExists()
    {
        using var connection = GetTestMasterDbSqlConnection();
        await connection.OpenAsync();
        await connection.ExecuteAsync("IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = 'ExerciseTestDB') CREATE DATABASE ExerciseTestDB;");
    }

    public static async Task DropTestTablesIfExists()
    {
        using var connection = GetTestDbSqlConnection();
        await connection.OpenAsync();
        await connection.ExecuteAsync(@"
            DROP TABLE IF EXISTS dbo.exercise_book_series_item;
            DROP TABLE IF EXISTS dbo.exercise_book_series;
            DROP TABLE IF EXISTS dbo.exercise_book_author;
            DROP TABLE IF EXISTS dbo.exercise_book;");
    }
}