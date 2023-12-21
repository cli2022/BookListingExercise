using Dapper;
using Xunit;

namespace BookListing.Repositories.Tests;

public class SchemaInitializerTests
{
    [Fact]
    public async Task InitializeSchema_WhenInvoked_TablesShouldBeCreated()
    {
        // Arrange
        await TestDbHelper.CreateTestDbIfNotExists();
        await TestDbHelper.DropTestTablesIfExists();
        var schemaInitializer = new SchemaInitializer(new TestSqlConnectionProvider());

        // Act
        schemaInitializer.EnsureInitialized();

        // Assert
        using var connection = TestDbHelper.GetTestDbSqlConnection();
        await connection.OpenAsync();
        Assert.Equal(1, connection.ExecuteScalar<int>("SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'exercise_book'"));
        Assert.Equal(1, connection.ExecuteScalar<int>("SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'exercise_book_author'"));
        Assert.Equal(1, connection.ExecuteScalar<int>("SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'exercise_book_series'"));
        Assert.Equal(1, connection.ExecuteScalar<int>("SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'exercise_book_series_item'"));
    }
}