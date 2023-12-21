import AddBook from '@/components/AddBook';
import ExistingBook from '@/components/ExistingBook';
import { Book } from '@/types/definitions';
import { InferGetStaticPropsType } from 'next';
import Head from 'next/head';
import Link from 'next/link';
import * as React from 'react';

// Page for Books list
export default function BookListing({
  books,
}: InferGetStaticPropsType<typeof getStaticProps>) {
  const [bookList, setBookList] = React.useState<Book[]>(books);

  const addBook = async (e: React.FormEvent, newBook: Book) => {
    e.preventDefault();
    await saveBook(newBook);
    location.reload();
  }

  const saveBook = async(book: Book) => {
    await fetch(process.env.API_BASE_URL + '/savebook', {
      method: 'POST',
      body: JSON.stringify(book),
      headers: {
        'Content-Type': 'application/json'
      }
    });
  }

  const deleteBook = async (id: number) => {
    await fetch(process.env.API_BASE_URL + '/books/' + id, { method: 'DELETE' });
    location.reload();
  }

  if (!bookList) return <h1>Loading...</h1>

  return (
    <div>
      <Head>
        <title>Book Listing Exercise</title>
      </Head>
      <main className='container'>
        <h1>Books</h1>
        <h2 className='link-series'>
          <Link href='/series'>Book Series &#8594;</Link>
        </h2>
        <AddBook addBook={addBook} />
        {bookList.map((book: Book) => (
          <ExistingBook key={book.id} book={book} deleteBook={deleteBook} saveBook={saveBook} />
        ))}
      </main>
    </div>
  )
}

export async function getStaticProps() {
  const response = await fetch(process.env.API_BASE_URL + '/books')
  const books: Book[] = await response.json()
  return {
    props: {
      books
    },
  }
}