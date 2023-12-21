namespace BookListing.Entities;
/// <summary>
/// Book record including title and info about its authors
/// </summary>
public class Book
{
    public int? Id { get; set; }
    public string? Title { get; set; }
    public List<BookAuthor>? Authors { get; set; }
}
