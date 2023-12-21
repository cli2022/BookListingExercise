using BookListing.Contracts.Managers;
using BookListing.Contracts.Repositories;
using BookListing.Entities;

namespace BookListing.Managers;
/// <summary>
/// Manager that is responsible for managing book series
/// </summary>
public class BookSeriesManager : IBookSeriesManager
{
    public readonly IBookSeriesRepository _bookSeriesRepository;

    public BookSeriesManager(IBookSeriesRepository bookSeriesRepository)
    {
        _bookSeriesRepository = bookSeriesRepository;
    }

    public async Task<List<BookSeries>> GetAll()
    {
        return await _bookSeriesRepository.GetAll();
    }

    public async Task<int> Save(BookSeries bookSeries)
    {
        return await _bookSeriesRepository.Save(bookSeries);
    }

    public async Task Delete(int bookSeriesId)
    {
        await _bookSeriesRepository.Delete(bookSeriesId);
    }
}