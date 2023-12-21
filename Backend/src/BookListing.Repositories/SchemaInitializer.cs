using System.Data.SqlClient;
using Dapper;

namespace BookListing.Repositories;
/// <summary>
/// Initializer that is used to initialize schema of tables in database
/// </summary>
public class SchemaInitializer : ISchemaInitializer
{
    private readonly ISqlConnectionProvider _sqlConnectionProvider;
    private readonly Lazy<bool> _initialized;
    
    public SchemaInitializer(ISqlConnectionProvider sqlConnectionProvider)
    {
        _sqlConnectionProvider = sqlConnectionProvider;
        _initialized = new Lazy<bool>(() =>
        {
            InitializeSchema();
            return true;
        });
    }

    public void EnsureInitialized()
    {
        _ = _initialized.Value;
    }

    public void InitializeSchema()
    {
        using var connection = _sqlConnectionProvider.GetSqlConnection();
        connection.Open();
        if (!DoesTableExist(connection, "dbo", "exercise_book"))
        {
            connection.Execute(@"
                CREATE TABLE dbo.exercise_book (
                    book_id INT IDENTITY(1, 1) NOT NULL PRIMARY KEY,
                    title NVARCHAR(900) NOT NULL
                );
                CREATE INDEX IX_book_title ON dbo.exercise_book (title);");
        }
        if (!DoesTableExist(connection, "dbo", "exercise_book_author"))
        {
            connection.Execute(@"
                CREATE TABLE dbo.exercise_book_author (
                    book_id INT FOREIGN KEY REFERENCES dbo.exercise_book NOT NULL,
                    first_name NVARCHAR(60) NOT NULL,
                    last_name NVARCHAR(60) NOT NULL,
                    PRIMARY KEY (book_id, first_name, last_name)
                );");
        }
        if (!DoesTableExist(connection, "dbo", "exercise_book_series"))
        {
            connection.Execute(@"
                CREATE TABLE dbo.exercise_book_series (
                    book_series_id INT IDENTITY(1, 1) NOT NULL PRIMARY KEY,
                    name NVARCHAR(900) NOT NULL
                );");
        }
        if (!DoesTableExist(connection, "dbo", "exercise_book_series_item"))
        {
            connection.Execute(@"
                CREATE TABLE dbo.exercise_book_series_item (
                    book_series_id INT FOREIGN KEY REFERENCES dbo.exercise_book_series NOT NULL,
                    book_id INT FOREIGN KEY REFERENCES dbo.exercise_book NOT NULL,
                    position INT NOT NULL,
                    PRIMARY KEY (book_series_id, book_id)
                );");
        }
    }

    private static bool DoesTableExist(SqlConnection connection, string schema, string table)
    {
        return connection.ExecuteScalar<int>($"SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = '{schema}' AND TABLE_NAME = '{table}'") > 0;
    }
}