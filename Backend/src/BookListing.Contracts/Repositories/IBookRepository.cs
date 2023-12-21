using BookListing.Entities;

namespace BookListing.Contracts.Repositories;
/// <summary>
/// Repository that is responsible for handling data persistence of books
/// </summary>
public interface IBookRepository
{
    Task<List<Book>> GetAll();
    Task<int> Save(Book book);
    Task Delete(int bookId);
}
