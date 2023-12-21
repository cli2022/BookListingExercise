using BookListing.Contracts.Managers;
using BookListing.Contracts.Repositories;
using BookListing.Entities;
using BookListing.Managers;
using BookListing.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Setup IoC
builder.Services.AddSingleton<IBookManager, BookManager>();
builder.Services.AddSingleton<IBookSeriesManager, BookSeriesManager>();
builder.Services.AddSingleton<IBookRepository, BookRepository>();
builder.Services.AddSingleton<IBookSeriesRepository, BookSeriesRepository>();
builder.Services.AddSingleton<ISchemaInitializer, SchemaInitializer>();
builder.Services.AddSingleton<ISqlConnectionProvider, SqlConnectionProvider>();

if (builder.Environment.IsDevelopment())
{
    // Allows backend to be accessed by frontend while it's hosted at different ports in dev environment
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAllForDev", builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
    });
}

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseCors("AllowAllForDev");
}

// Setup API routes
app.MapGet("/books", async (IBookManager manager) => await manager.GetAll());

app.MapPost("/savebook", async (Book book, IBookManager manager) => await manager.Save(book));

app.MapDelete("/books/{bookId}", async (int bookId, IBookManager manager) => await manager.Delete(bookId));

app.MapGet("/series", async (IBookSeriesManager manager) => await manager.GetAll());

app.MapPost("/saveseries", async (BookSeries series, IBookSeriesManager manager) => await manager.Save(series));

app.MapDelete("/series/{seriesId}", async (int seriesId, IBookSeriesManager manager) => await manager.Delete(seriesId));

app.MapGet("/", () => "Book Listing API is running");

app.Run();
