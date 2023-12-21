using BookListing.Entities;

namespace BookListing.Contracts.Managers;
/// <summary>
/// Manager that is responsible for managing books
/// </summary>
public interface IBookManager
{
    Task<List<Book>> GetAll();
    Task<int> Save(Book book);
    Task Delete(int bookId);
}