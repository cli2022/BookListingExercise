using BookListing.Entities;
using Dapper;
using Xunit;

namespace BookListing.Repositories.Tests;

public class BookRepositoryTests
{
    [Fact]
    public async Task GetAll_BooksInDb_BooksShouldBeReturned()
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
            INSERT INTO dbo.exercise_book_author (book_id, first_name, last_name) VALUES (1, 'FA', 'LA'), (2, 'FB', 'LB'), (3, 'FC', 'LC'), (3, 'FD', 'LD');
            SET IDENTITY_INSERT dbo.exercise_book OFF;");

        // Act
        var results = await repository.GetAll();

        // Assert
        Assert.Equal(3, results.Count);
        var book1 = results.FirstOrDefault(x => x.Id == 1);
        Assert.NotNull(book1);
        Assert.Equal("Book A", book1!.Title);
        Assert.Contains(book1!.Authors, x => x.FirstName == "FA" && x.LastName == "LA");
        var book2 = results.FirstOrDefault(x => x.Id == 2);
        Assert.NotNull(book2);
        Assert.Equal("Book B", book2!.Title);
        Assert.Contains(book2!.Authors, x => x.FirstName == "FB" && x.LastName == "LB");
        var book3 = results.FirstOrDefault(x => x.Id == 3);
        Assert.NotNull(book3);
        Assert.Equal("Book C", book3!.Title);
        Assert.Contains(book3!.Authors, x => x.FirstName == "FC" && x.LastName == "LC");
        Assert.Contains(book3!.Authors, x => x.FirstName == "FD" && x.LastName == "LD");
    }

    [Fact]
    public async Task Save_NewBook_NewBookShouldBeSavedToDb()
    {
        // Arrange
        await TestDbHelper.CreateTestDbIfNotExists();
        await TestDbHelper.DropTestTablesIfExists();
        var repository = GetRepositoryUnderTest();
        var newBook = new Book
        {
            Title = "Book 5",
            Authors = new List<BookAuthor>
            {
                new() { FirstName = "FE", LastName = "LE" }
            }
        };

        // Act
        await repository.Save(newBook);

        // Assert
        using var connection = TestDbHelper.GetTestDbSqlConnection();
        await connection.OpenAsync();
        Assert.Equal(1, connection.ExecuteScalar<int>(@"SELECT COUNT(*) FROM dbo.exercise_book WHERE title = 'Book 5'"));
        Assert.Equal(1, connection.ExecuteScalar<int>(@"SELECT COUNT(*) FROM dbo.exercise_book_author WHERE first_name = 'FE' AND last_name = 'LE'"));
    }

    [Fact]
    public async Task Save_ExistingBook_ExistingBookShouldBeUpdatedInDb()
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
            INSERT INTO dbo.exercise_book (book_id, title) VALUES (1, 'Book A');
            INSERT INTO dbo.exercise_book_author (book_id, first_name, last_name) VALUES (1, 'FA', 'LA');
            SET IDENTITY_INSERT dbo.exercise_book OFF;");
        }
        
        var existingBook = new Book
        {
            Id = 1,
            Title = "Book E",
            Authors = new List<BookAuthor>
            {
                new() { FirstName = "FF", LastName = "LF" }
            }
        };

        // Act
        await repository.Save(existingBook);

        // Assert
        using (var connection = TestDbHelper.GetTestDbSqlConnection())
        {
            await connection.OpenAsync();
            Assert.Equal(1, connection.ExecuteScalar<int>(@"SELECT COUNT(*) FROM dbo.exercise_book WHERE book_id = 1 AND title = 'Book E'"));
            Assert.Equal(1, connection.ExecuteScalar<int>(@"SELECT COUNT(*) FROM dbo.exercise_book_author WHERE book_id = 1 AND first_name = 'FF' AND last_name = 'LF'"));
        }
    }

    [Fact]
    public async Task Delete_ExistingBook_ExistingBookShouldBeDeletedFromDb()
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
            INSERT INTO dbo.exercise_book (book_id, title) VALUES (1, 'Book A');
            INSERT INTO dbo.exercise_book_author (book_id, first_name, last_name) VALUES (1, 'FA', 'LA');
            SET IDENTITY_INSERT dbo.exercise_book OFF;");
        }

        // Act
        await repository.Delete(1);

        // Assert
        using (var connection = TestDbHelper.GetTestDbSqlConnection())
        {
            await connection.OpenAsync();
            Assert.Equal(0, connection.ExecuteScalar<int>(@"SELECT COUNT(*) FROM dbo.exercise_book WHERE book_id = 1"));
            Assert.Equal(0, connection.ExecuteScalar<int>(@"SELECT COUNT(*) FROM dbo.exercise_book_author WHERE book_id = 1"));
        }
    }

    private static BookRepository GetRepositoryUnderTest()
    {
        return new BookRepository(new TestSqlConnectionProvider(), new SchemaInitializer(new TestSqlConnectionProvider()));
    }
}