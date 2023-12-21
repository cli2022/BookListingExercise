using BookListing.Contracts.Managers;
using BookListing.Contracts.Repositories;
using BookListing.Entities;

namespace BookListing.Managers;
/// <summary>
/// Manager that is responsible for managing books
/// </summary>
public class BookManager : IBookManager
{
    private readonly IBookRepository _bookRepository;

    public BookManager(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public async Task<List<Book>> GetAll()
    {    
        return await _bookRepository.GetAll();
    }

    public async Task<int> Save(Book book)
    {
        return await _bookRepository.Save(book);
    }

    public async Task Delete(int bookId)
    {
        await _bookRepository.Delete(bookId);
    }
}
