using BookListing.Contracts.Repositories;
using BookListing.Entities;
using Dapper;

namespace BookListing.Repositories;
/// <summary>
/// Repository that is responsible for handling data persistence of books
/// </summary>
public class BookRepository : IBookRepository
{
    private readonly ISqlConnectionProvider _sqlConnectionProvider;

    public BookRepository(ISqlConnectionProvider sqlConnectionProvider, ISchemaInitializer schemaInitializer)
    {
        _sqlConnectionProvider = sqlConnectionProvider;
        schemaInitializer.EnsureInitialized();
    }

    public async Task<List<Book>> GetAll()
    {
        using var connection = _sqlConnectionProvider.GetSqlConnection();
        await connection.OpenAsync();
        var bookById = new Dictionary<int, Book>();
        return (await connection.QueryAsync<BookAuthor, Book, Book>(@"
            SELECT
                a.first_name AS FirstName,
                a.last_name AS LastName,
                b.book_id AS Id,
                b.title AS Title
            FROM dbo.exercise_book b
            LEFT JOIN dbo.exercise_book_author a ON a.book_id = b.book_id;",
            (author, book) =>
            {
                if (!bookById.TryGetValue(book.Id!.Value, out var thisBook))
                {
                    bookById[book.Id!.Value] = book;
                    thisBook = book;
                    thisBook.Authors = new List<BookAuthor>();
                }
                if (!(author.FirstName == null && author.LastName == null))
                {
                    thisBook.Authors!.Add(author);
                }
                return thisBook;
            })).DistinctBy(x => x.Id!.Value).ToList();
    }

    public async Task<int> Save(Book book)
    {
        using var connection = _sqlConnectionProvider.GetSqlConnection();
        await connection.OpenAsync();
        using var transaction = await connection.BeginTransactionAsync();

        int bookId;
        if (book.Id == null)
        {
            bookId = await connection.ExecuteScalarAsync<int>(@"
                DECLARE @InsertedBook TABLE (Id INT);
                INSERT INTO dbo.exercise_book (title) OUTPUT INSERTED.book_id INTO @InsertedBook VALUES (@Title);
                SELECT TOP 1 Id FROM @InsertedBook;",
                new { book.Title }, transaction);
        }
        else
        {
            bookId = book.Id.Value;
            await connection.ExecuteAsync(@"
                UPDATE dbo.exercise_book SET title = @Title WHERE book_id = @Id;",
                new { Id = bookId, book.Title }, transaction);
        }

        var authorsParameters = (book.Authors ?? new List<BookAuthor>()).Select((x, i) =>
            (FirstNameParameter: new KeyValuePair<string, object?>($"@FirstName{i}", x.FirstName ?? string.Empty),
            LastNameParameter: new KeyValuePair<string, object?>($"@LastName{i}", x.LastName ?? string.Empty)));

        await connection.ExecuteAsync(@$"
            MERGE INTO dbo.exercise_book_author AS target_values
            USING (VALUES {(authorsParameters.Any() ? string.Join(", ", authorsParameters.Select(x => $"(@BookId, {x.FirstNameParameter.Key}, {x.LastNameParameter.Key})")) : "(null, null, null)")}) AS source_values(book_id, first_name, last_name)
            ON source_values.book_id = target_values.book_id AND source_values.first_name = target_values.first_name AND source_values.last_name = target_values.last_name
            WHEN NOT MATCHED BY TARGET AND source_values.book_id IS NOT NULL THEN INSERT(book_id, first_name, last_name) VALUES (source_values.book_id, source_values.first_name, source_values.last_name)
            WHEN NOT MATCHED BY SOURCE AND target_values.book_id = @BookId THEN DELETE;",
            authorsParameters.SelectMany(x => new [] { x.FirstNameParameter, x.LastNameParameter }).Concat(new [] { new KeyValuePair<string, object?>("@BookId", bookId) }).ToDictionary(x => x.Key, x => x.Value),
            transaction);

        await transaction.CommitAsync();
        return bookId;
    }

    public async Task Delete(int bookId)
    {
        using var connection = _sqlConnectionProvider.GetSqlConnection();
        await connection.OpenAsync();
        using var transaction = await connection.BeginTransactionAsync();

        await connection.ExecuteAsync(@"
            DELETE FROM dbo.exercise_book_series_item WHERE book_id = @Id;",
            new { Id = bookId }, transaction);

        await connection.ExecuteAsync(@"
            DELETE FROM dbo.exercise_book_author WHERE book_id = @Id;",
            new { Id = bookId }, transaction);

        await connection.ExecuteAsync(@"
            DELETE FROM dbo.exercise_book WHERE book_id = @Id;",
            new { Id = bookId }, transaction);

        await transaction.CommitAsync();
    }
}
