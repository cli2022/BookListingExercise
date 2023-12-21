namespace BookListing.Entities;
/// <summary>
/// A book series including series name and info about items in the series
/// </summary>
public class BookSeries
{
    public int? Id { get; set; }
    public string? Name { get; set; }
    public List<BookSeriesItem>? Books { get; set; }
}