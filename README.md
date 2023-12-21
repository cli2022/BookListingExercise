# Book Listing Exercise
## Backend
### General Info
- `BookListing.Api.Host` (.NET 6 web application that exposes the following REST API endpoints)
  - `/books` (via HTTP GET) for getting existing books.
  - `/savebook` (via HTTP POST) for saving a new book or saving updates to an existing book.
  - `/books/{bookId}` (via HTTP DELETE) for deleting an existing book.
  - `/series` (via HTTP GET) for getting existing book series.
  - `/saveseries` (via HTTP POST) for saving a new book series or saving updates to an existing book series.
  - `/series/{seriesId}` (via HTTP DELETE) for deleting an existing book series.
- Uses SQL Server database for data storage. The backend application requires a connection string for a SQL Server instance with an existing database to be configured in its appsettings.
  - The backend application will auto create the following tables in the specified database at runtime.
    - `dbo.exercise_book`
	- `dbo.exercise_book_author`
	- `dbo.exercise_book_series`
	- `dbo.exercise_book_series_item`
  - The schema of these tables are designed while taking the following realistic factors into consideration.
    - It's possible that two different books have identical title.
	- It's possible that two different authors (of different books) have the same first name and last name.
	- It's possible that two different book series have identical series name.
### How to Run (in dev environment)
- Configure the connection string for `DB` in `Backend/src/BookListing.Api.Host/appsettings.Development.json`.
  - Adjust the SQL Server instance to set to an existing instance of SQL Server, the default setting assumes `localhost`.
  - Adjust the database name to set to an existing database, the default assumes `ExerciseDB`.
    - Note the database can be any existing database (for example a newly created empty database), as the backend application will auto create schema of needed tables in the specified database at runtime.
- Open `Backend/BookListing.sln` in VS Code or Visual Studio and start running `BookListing.Api.Host` project.
  - By default the API would be running at `https://localhost:7138` and `http://localhost:5258` in dev environment.
## Frontend
### General Info
- `book-listing-app` (React Next.js TypeScript application that provides web screens for managing books and book series).
- Consumes endpoints exposed by BookListing.Api.Host.
### How to Run (in dev environment)
- Configure `API_BASE_URL` in `Frontend/src/book-listing-app/next.config.js`.
  - By default, this is set to `http://localhost:5258`, which points to `BookListing.Api.Host` hosted in dev environment.
- In a terminal, navigate to `Frontend/src/book-listing-app` folder, then execute `npm run dev` to run the frontend.
  - Note `BookListing.Api.Host` needs to be started before running the frontend.
  - This requires Node.js to host the frontend app in dev environment, by default hosted at `http://localhost:3000`.
## Unit Tests (with database integration) for Backend
### General Info
- `BookListing.Repositories.Tests` contains unit tests (with integration to test against a test database).
  - Note it requires connection string for test database to be configured in `Backend/tests/BookListing.Repositories.Tests/appsettings.json`.
    - Configure the connection string for `TestMasterDB` and `TestDB` to set to an existing instance of SQL Server, the default setting assumes `localhost`.
	- `TestMasterDB` connection string needs to refer to `master` database (which is the connection used by the tests to auto create `ExerciseTestDB` test database).
	- `TestDB` connection string needs to refer to `ExerciseTestDB` database (which is the name of test database assumed by the tests).
  - It contains tests which test repository that is responsible for handling data persistence of books, against a test database.
  - It contains tests which test repository that is responsible for handling data persistence of book series, against a test database.