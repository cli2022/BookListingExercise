namespace BookListing.Repositories;
/// <summary>
/// Initializer that is used to initialize schema of tables in database
/// </summary>
public interface ISchemaInitializer
{
    void EnsureInitialized();
}