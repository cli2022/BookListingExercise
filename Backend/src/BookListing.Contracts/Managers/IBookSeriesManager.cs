using BookListing.Entities;

namespace BookListing.Contracts.Managers;
/// <summary>
/// Manager that is responsible for managing book series
/// </summary>
public interface IBookSeriesManager
{
    Task<List<BookSeries>> GetAll();
    Task<int> Save(BookSeries bookSeries);
    Task Delete(int bookSeriesId);
}