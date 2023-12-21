namespace BookListing.Entities;
/// <summary>
/// Book series item which indicates a book and its position in a series
/// </summary>
public class BookSeriesItem
{
    public int? BookId { get; set; }
    public int? Position { get; set; }
}