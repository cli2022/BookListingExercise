using BookListing.Contracts.Repositories;
using BookListing.Entities;
using Dapper;

namespace BookListing.Repositories;
/// <summary>
/// Repository that is responsible for handling data persistence of book series
/// </summary>
public class BookSeriesRepository : IBookSeriesRepository
{
    private readonly ISqlConnectionProvider _sqlConnectionProvider;

    public BookSeriesRepository(ISqlConnectionProvider sqlConnectionProvider, ISchemaInitializer schemaInitializer)
    {
        _sqlConnectionProvider = sqlConnectionProvider;
        schemaInitializer.EnsureInitialized();
    }

    public async Task<List<BookSeries>> GetAll()
    {
        using var connection = _sqlConnectionProvider.GetSqlConnection();
        await connection.OpenAsync();
        var seriesById = new Dictionary<int, BookSeries>();
        return (await connection.QueryAsync<BookSeriesItem, BookSeries, BookSeries>(@"
            SELECT
                si.book_id AS BookId,
                si.position AS Position,
                s.book_series_id AS Id,
                s.name AS Name
            FROM dbo.exercise_book_series s
            LEFT JOIN dbo.exercise_book_series_item si ON si.book_series_id = s.book_series_id;",
            (seriesItem, series) =>
            {
                if (!seriesById.TryGetValue(series.Id!.Value, out var thisSeries))
                {
                    seriesById[series.Id!.Value] = series;
                    thisSeries = series;
                    thisSeries.Books = new List<BookSeriesItem>();
                }
                if (!(seriesItem.BookId == null && seriesItem.Position == null))
                {
                    thisSeries.Books!.Add(seriesItem);
                }
                return thisSeries;
            })).DistinctBy(x => x.Id!.Value).ToList();
    }

    public async Task<int> Save(BookSeries bookSeries)
    {
        using var connection = _sqlConnectionProvider.GetSqlConnection();
        await connection.OpenAsync();
        using var transaction = await connection.BeginTransactionAsync();

        int seriesId;
        if (bookSeries.Id == null)
        {
            seriesId = await connection.ExecuteScalarAsync<int>(@"
                DECLARE @InsertedSeries TABLE (Id INT);
                INSERT INTO dbo.exercise_book_series (name) OUTPUT INSERTED.book_series_id INTO @InsertedSeries VALUES (@Name);
                SELECT TOP 1 Id FROM @InsertedSeries;",
                new { bookSeries.Name }, transaction);
        }
        else
        {
            seriesId = bookSeries.Id.Value;
            await connection.ExecuteAsync(@"
                UPDATE dbo.exercise_book_series SET name = @Name WHERE book_series_id = @Id;",
                new { Id = seriesId, bookSeries.Name }, transaction);
        }

        var seriesItemsParameters = (bookSeries.Books ?? new List<BookSeriesItem>()).Select((x, i) =>
            (BookIdParameter: new KeyValuePair<string, object?>($"@BookId{i}", x.BookId!.Value),
            PositionParameter: new KeyValuePair<string, object?>($"@Position{i}", x.Position!.Value)));

        await connection.ExecuteAsync(@$"
            MERGE INTO dbo.exercise_book_series_item AS target_values
            USING (VALUES {(seriesItemsParameters.Any() ? string.Join(", ", seriesItemsParameters.Select(x => $"(@BookSeriesId, {x.BookIdParameter.Key}, {x.PositionParameter.Key})")) : "(null, null, null)")}) AS source_values(book_series_id, book_id, position)
            ON source_values.book_series_id = target_values.book_series_id AND source_values.book_id = target_values.book_id
            WHEN MATCHED AND source_values.position <> target_values.position THEN UPDATE SET target_values.position = source_values.position
            WHEN NOT MATCHED BY TARGET AND source_values.book_series_id IS NOT NULL THEN INSERT(book_series_id, book_id, position) VALUES (source_values.book_series_id, source_values.book_id, source_values.position)
            WHEN NOT MATCHED BY SOURCE AND target_values.book_series_id = @BookSeriesId THEN DELETE;",
            seriesItemsParameters.SelectMany(x => new [] { x.BookIdParameter, x.PositionParameter }).Concat(new [] { new KeyValuePair<string, object?>("@BookSeriesId", seriesId) }).ToDictionary(x => x.Key, x => x.Value),
            transaction);

        await transaction.CommitAsync();
        return seriesId;
    }

    public async Task Delete(int bookSeriesId)
    {
        using var connection = _sqlConnectionProvider.GetSqlConnection();
        await connection.OpenAsync();
        using var transaction = await connection.BeginTransactionAsync();

        await connection.ExecuteAsync(@"
            DELETE FROM dbo.exercise_book_series_item WHERE book_series_id = @Id;",
            new { Id = bookSeriesId }, transaction);

        await connection.ExecuteAsync(@"
            DELETE FROM dbo.exercise_book_series WHERE book_series_id = @Id;",
            new { Id = bookSeriesId }, transaction);

        await transaction.CommitAsync();
    }
}