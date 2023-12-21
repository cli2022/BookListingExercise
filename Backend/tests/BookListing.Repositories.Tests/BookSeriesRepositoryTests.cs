using BookListing.Entities;
using Dapper;
using Xunit;

namespace BookListing.Repositories.Tests;

public class BookSeriesRepositoryTests
{
    [Fact]
    public async Task GetAll_SeriesInDb_SeriesShouldBeReturned()
    {
        // Arrange
        await TestDbHelper.CreateTestDbIfNotExists();
        await TestDbHelper.DropTestTablesIfExists();
        var repository = GetRepositoryUnderTest();
        using var connection = TestDbHelper.GetTestDbSqlConnection();
        await connection.OpenAsync();
        await connection.ExecuteAsync(@"
            SET IDENTITY_INSERT dbo.exercise_book ON;
            INSERT INTO dbo.exercise_book (book_id, title) VALUES (1, 'Book A'), (2, 'Book B'), (3, 'Book C');
            SET IDENTITY_INSERT dbo.exercise_book OFF;
            SET IDENTITY_INSERT dbo.exercise_book_series ON;
            INSERT INTO dbo.exercise_book_series (book_series_id, name) VALUES (1, 'S1'), (2, 'S2');
            SET IDENTITY_INSERT dbo.exercise_book_series OFF;
            INSERT INTO dbo.exercise_book_series_item (book_series_id, book_id, position) VALUES (1, 1, 1), (1, 3, 2), (2, 2, 1);");

        // Act
        var results = await repository.GetAll();

        // Assert
        Assert.Equal(2, results.Count);
        var series1 = results.FirstOrDefault(x => x.Id == 1);
        Assert.NotNull(series1);
        Assert.Equal("S1", series1!.Name);
        Assert.Contains(series1!.Books, x => x.BookId == 1 && x.Position == 1);
        Assert.Contains(series1!.Books, x => x.BookId == 3 && x.Position == 2);
        var series2 = results.FirstOrDefault(x => x.Id == 2);
        Assert.NotNull(series2);
        Assert.Equal("S2", series2!.Name);
        Assert.Contains(series2!.Books, x => x.BookId == 2 && x.Position == 1);
    }

    [Fact]
    public async Task Save_NewSeries_NewSeriesShouldBeSavedToDb()
    {
        // Arrange
        await TestDbHelper.CreateTestDbIfNotExists();
        await TestDbHelper.DropTestTablesIfExists();
        var repository = GetRepositoryUnderTest();
        using (var connection = TestDbHelper.GetTestDbSqlConnection())
        {
            await connection.OpenAsync();
            await connection.ExecuteAsync(@"
            SET IDENTITY_INSERT dbo.exercise_book ON;
            INSERT INTO dbo.exercise_book (book_id, title) VALUES (2, 'Book B');
            INSERT INTO dbo.exercise_book_author (book_id, first_name, last_name) VALUES (2, 'FA', 'LA');
            SET IDENTITY_INSERT dbo.exercise_book OFF;");
        }
        var newSeries = new BookSeries
        {
            Name = "S7",
            Books = new List<BookSeriesItem>
            {
                new() { BookId = 2, Position = 1 }
            }
        };

        // Act
        await repository.Save(newSeries);

        // Assert
        using (var connection = TestDbHelper.GetTestDbSqlConnection())
        {
            await connection.OpenAsync();
            Assert.Equal(1, connection.ExecuteScalar<int>(@"SELECT COUNT(*) FROM dbo.exercise_book_series WHERE name = 'S7'"));
            Assert.Equal(1, connection.ExecuteScalar<int>(@"SELECT COUNT(*) FROM dbo.exercise_book_series_item WHERE book_id = 2 AND position = 1"));
        }
    }

    [Fact]
    public async Task Save_ExistingSeries_ExistingSeriesShouldBeUpdatedInDb()
    {
        // Arrange
        await TestDbHelper.CreateTestDbIfNotExists();
        await TestDbHelper.DropTestTablesIfExists();
        var repository = GetRepositoryUnderTest();
        using (var connection = TestDbHelper.GetTestDbSqlConnection())
        {
            await connection.OpenAsync();
            await connection.ExecuteAsync(@"
            SET IDENTITY_INSERT dbo.exercise_book ON;
            INSERT INTO dbo.exercise_book (book_id, title) VALUES (1, 'Book A'), (2, 'Book B'), (3, 'Book C');
            SET IDENTITY_INSERT dbo.exercise_book OFF;
            SET IDENTITY_INSERT dbo.exercise_book_series ON;
            INSERT INTO dbo.exercise_book_series (book_series_id, name) VALUES (1, 'S1');
            SET IDENTITY_INSERT dbo.exercise_book_series OFF;
            INSERT INTO dbo.exercise_book_series_item (book_series_id, book_id, position) VALUES (1, 1, 1), (1, 3, 2);");
        }
        
        var existingSeries = new BookSeries
        {
            Id = 1,
            Name = "S7",
            Books = new List<BookSeriesItem>
            {
                new() { BookId = 2, Position = 1 },
                new() { BookId = 1, Position = 2 },
                new() { BookId = 3, Position = 3 }
            }
        };

        // Act
        await repository.Save(existingSeries);

        // Assert
        using (var connection = TestDbHelper.GetTestDbSqlConnection())
        {
            await connection.OpenAsync();
            Assert.Equal(1, connection.ExecuteScalar<int>(@"SELECT COUNT(*) FROM dbo.exercise_book_series WHERE book_series_id = 1 AND name = 'S7'"));
            Assert.Equal(3, connection.ExecuteScalar<int>(@"SELECT COUNT(*) FROM dbo.exercise_book_series_item WHERE book_series_id = 1"));
            Assert.Equal(1, connection.ExecuteScalar<int>(@"SELECT COUNT(*) FROM dbo.exercise_book_series_item WHERE book_series_id = 1 AND book_id = 2 AND position = 1"));
            Assert.Equal(1, connection.ExecuteScalar<int>(@"SELECT COUNT(*) FROM dbo.exercise_book_series_item WHERE book_series_id = 1 AND book_id = 1 AND position = 2"));
            Assert.Equal(1, connection.ExecuteScalar<int>(@"SELECT COUNT(*) FROM dbo.exercise_book_series_item WHERE book_series_id = 1 AND book_id = 3 AND position = 3"));
        }
    }

    [Fact]
    public async Task Delete_ExistingSeries_ExistingSeriesShouldBeDeletedFromDb()
    {
        // Arrange
        await TestDbHelper.CreateTestDbIfNotExists();
        await TestDbHelper.DropTestTablesIfExists();
        var repository = GetRepositoryUnderTest();
        using (var connection = TestDbHelper.GetTestDbSqlConnection())
        {
            await connection.OpenAsync();
            await connection.ExecuteAsync(@"
            SET IDENTITY_INSERT dbo.exercise_book ON;
            INSERT INTO dbo.exercise_book (book_id, title) VALUES (1, 'Book A'), (2, 'Book B'), (3, 'Book C');
            SET IDENTITY_INSERT dbo.exercise_book OFF;
            SET IDENTITY_INSERT dbo.exercise_book_series ON;
            INSERT INTO dbo.exercise_book_series (book_series_id, name) VALUES (1, 'S1');
            SET IDENTITY_INSERT dbo.exercise_book_series OFF;
            INSERT INTO dbo.exercise_book_series_item (book_series_id, book_id, position) VALUES (1, 1, 1), (1, 3, 2);");
        }

        // Act
        await repository.Delete(1);

        // Assert
        using (var connection = TestDbHelper.GetTestDbSqlConnection())
        {
            await connection.OpenAsync();
            Assert.Equal(0, connection.ExecuteScalar<int>(@"SELECT COUNT(*) FROM dbo.exercise_book_series WHERE book_series_id = 1"));
            Assert.Equal(0, connection.ExecuteScalar<int>(@"SELECT COUNT(*) FROM dbo.exercise_book_series_item WHERE book_series_id = 1"));
        }
    }

    private static BookSeriesRepository GetRepositoryUnderTest()
    {
        return new BookSeriesRepository(new TestSqlConnectionProvider(), new SchemaInitializer(new TestSqlConnectionProvider()));
    }
}