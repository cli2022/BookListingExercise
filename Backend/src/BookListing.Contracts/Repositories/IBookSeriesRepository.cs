using BookListing.Entities;

namespace BookListing.Contracts.Repositories;
/// <summary>
/// Repository that is responsible for handling data persistence of book series
/// </summary>
public interface IBookSeriesRepository
{
    Task<List<BookSeries>> GetAll();
    Task<int> Save(BookSeries bookSeries);
    Task Delete(int bookSeriesId);
}