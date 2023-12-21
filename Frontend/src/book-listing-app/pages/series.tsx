import AddSeries from '@/components/AddSeries';
import ExistingSeries from '@/components/ExistingSeries';
import { Book, BookSeries } from '@/types/definitions';
import { InferGetStaticPropsType } from 'next';
import Head from 'next/head';
import Link from 'next/link';
import * as React from 'react';

// Page for Book Series list
export default function SeriesListing({
  allSeries,
  books
}: InferGetStaticPropsType<typeof getStaticProps>) {
  const [seriesList, setSeriesList] = React.useState<BookSeries[]>(allSeries);
  const [searchResults, setSearchResults] = React.useState<string[]>();

  const addSeries = async (e: React.FormEvent, newSeries: BookSeries) => {
    e.preventDefault();
    await saveSeries(newSeries);
    location.reload();
  }

  const saveSeries = async(series: BookSeries) => {
    await fetch(process.env.API_BASE_URL + '/saveseries', {
      method: 'POST',
      body: JSON.stringify(series),
      headers: {
        'Content-Type': 'application/json'
      }
    });
  }

  const deleteSeries = async (id: number) => {
    await fetch(process.env.API_BASE_URL + '/series/' + id, { method: 'DELETE' });
    location.reload();
  }

  const handleBookSearch = (e: React.FormEvent<HTMLInputElement>) => {
    const searchText = e.currentTarget.value.toLowerCase();
    if (searchText.trim() === '') {
      setSearchResults(undefined);
    } else {
      const matchedBooks = books.filter(book => book.title.toLowerCase() === searchText);
      let results = [] as string[];
      allSeries.forEach(series => {
        matchedBooks.forEach(book => {
          const matchedItem = series.books.find(item => item.bookId === book.id);
          if (matchedItem !== undefined) {
            results = [...results, 'At position ' + matchedItem.position + ' of series ' + series.name];
          }
        });
      });
      setSearchResults(results);
    }
  };

  if (!seriesList) return <h1>Loading...</h1>

  return (
    <div>
      <Head>
        <title>Book Listing Exercise</title>
      </Head>
      <main className='container'>
        <h1>Book Series</h1>
        <h2 className='link-books'>
          <Link href='/'>&#8592; Books</Link>
        </h2>
        <div className='book-search'>
          <input type='text' placeholder='Search in series by book title...' onChange={handleBookSearch} id='bookSearch'/>
          {searchResults !== undefined && searchResults.length > 0 &&
            <div className='book-search-result'>
              <p>Found this book in the following series:</p>
              <ul>
                {searchResults.map((result: string, i) => (
                  <li key={i}>{result}</li>
                ))}
              </ul>
            </div>
          }
          {searchResults !== undefined && searchResults.length === 0 &&
            <div className='book-search-result'>
              <p>No results found.</p>
            </div>
          }
        </div>
        <AddSeries addSeries={addSeries} books={books} />
        {seriesList.map((series: BookSeries) => (
          <ExistingSeries key={series.id} series={series} books={books} deleteSeries={deleteSeries} saveSeries={saveSeries} />
        ))}
      </main>
    </div>
  )
}

export async function getStaticProps() {
  const seriesResponse = await fetch(process.env.API_BASE_URL + '/series')
  const allSeries: BookSeries[] = await seriesResponse.json()
  const booksResponse = await fetch(process.env.API_BASE_URL + '/books')
  const books: Book[] = await booksResponse.json()
  return {
    props: {
      allSeries,
      books
    },
  }
}