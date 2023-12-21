import { Book, BookSeries, BookSeriesItem } from '@/types/definitions';
import * as React from 'react';

type Props = {
  addSeries: (e: React.FormEvent, newSeries: BookSeries) => void,
  books: Book[]
};

// Component for adding a new book series
const AddSeries: React.FC<Props> = ({ addSeries, books }) => {
  const [name, setName] = React.useState<string>();
  const [seriesItems, setSeriesItems] = React.useState<BookSeriesItem[]>([]);

  const handleNameChange = (e: React.FormEvent<HTMLInputElement>) => {
    setName(e.currentTarget.value);
  };

  const handleSeriesItemPositionChange = (e: React.FormEvent<HTMLInputElement>) => {
    const [, suffix] = e.currentTarget.id.split('_');
    const itemIndex = Number(suffix);
    seriesItems[itemIndex].position = Number(e.currentTarget.value);
  };

  const handleSeriesItemBookChange = (e: React.FormEvent<HTMLSelectElement>) => {
    const [, suffix] = e.currentTarget.id.split('_');
    const itemIndex = Number(suffix);
    seriesItems[itemIndex].bookId = Number(e.currentTarget.value);
  };

  const handleAddSeriesItem = () => {
    const newSeriesItem = { position: seriesItems.length + 1 } as BookSeriesItem;
    setSeriesItems([...seriesItems, newSeriesItem]);
  };

  const handleAddSeries = (e: React.FormEvent<HTMLFormElement>) => {
    const newSeries = { name: name, books: seriesItems } as BookSeries
    newSeries.books = newSeries.books.filter(item => item.bookId > 0).sort((a, b) => a.position - b.position);
    newSeries.books.forEach((item, i) => {
        item.position = i + 1
    });
    addSeries(e, newSeries);
  };

  return (
    <form className='form' onSubmit={(e) => handleAddSeries(e)}>
      <div>
        <div className='form-field'>
          <label>Series Name</label>
          <input onChange={handleNameChange} type='text' id='name' />
        </div>
        {seriesItems?.map((item: BookSeriesItem, i: number) => (
          <div key={i}>
            <div className='form-field'>
              <label>Book Position:</label>
              <input className='position' defaultValue={item.position} onChange={handleSeriesItemPositionChange} type='text' id={`position_${i}`} />
            </div>
            <div className='form-field'>
              <label>Book:</label>
              <select className='select-book' onChange={handleSeriesItemBookChange} id={`bookId_${i}`}>
                <option value='-1'></option>
                {books.map((book: Book) => (
                    <option key={`${book.id}`} value={`${book.id}`}>{book.title + (book.authors.length > 0 ? ' - ' + book.authors.map(author => author.firstName + ' ' + author.lastName).join(', ') : '')}</option>
                ))}
              </select>
            </div>
          </div>
        ))}
      </div>
      <button type='button' onClick={handleAddSeriesItem}>+ Book</button>
      <button className='form-button' disabled={name === undefined || name.trim() === '' ? true : false}>Add New Series</button>
    </form>
  )
};

export default AddSeries;