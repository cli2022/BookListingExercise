import { Book, BookSeries, BookSeriesItem } from '@/types/definitions';
import * as React from 'react';

type Props = {
  series: BookSeries,
  books: Book[]
  deleteSeries: (id: number) => void,
  saveSeries: (series: BookSeries) => void
};

// Component for rendering and editing an existing book series
const ExistingSeries: React.FC<Props> = ({ series, books, deleteSeries, saveSeries }) => {
  const [isEditing, setIsEditing] = React.useState<boolean>(false);
  const [seriesItems, setSeriesItems] = React.useState<BookSeriesItem[]>(series.books.sort((a, b) => a.position - b.position));

  const handleNameChange = (e: React.FormEvent<HTMLInputElement>) => {
    series.name = e.currentTarget.value;
  };

  const handleSeriesItemPositionChange = (e: React.FormEvent<HTMLInputElement>) => {
    const [, suffix] = e.currentTarget.id.split('_');
    const itemIndex = Number(suffix);
    seriesItems[itemIndex].position = Number(e.currentTarget.value);
    series.books = seriesItems;
  };

  const handleSeriesItemBookChange = (e: React.FormEvent<HTMLSelectElement>) => {
    const [, suffix] = e.currentTarget.id.split('_');
    const itemIndex = Number(suffix);
    seriesItems[itemIndex].bookId = Number(e.currentTarget.value);
    series.books = seriesItems;
  };

  const handleAddSeriesItem = () => {
    const newSeriesItem = { position: seriesItems.length + 1 } as BookSeriesItem;
    setSeriesItems([...seriesItems, newSeriesItem]);
  };

  const handleSaveSeries = () => {
    series.books = series.books.filter(item => item.bookId > 0).sort((a, b) => a.position - b.position);
    series.books.forEach((item, i) => {
        item.position = i + 1
    });
    saveSeries(series);
    setSeriesItems(series.books);
    setIsEditing(false);
  };

  return (
    <div className='card'>
      {!isEditing &&
        <div className='card-body'>
          <h1 className='card-body-title'>{series.name}</h1>
        </div>
      }
      {isEditing &&
        <div className='card-edit-body'>
          <div>
            <label>Series Name</label>
            <input defaultValue={series.name} onChange={handleNameChange} type='text' id='name' />
          </div>
          {seriesItems?.map((item: BookSeriesItem, i: number) => (
            <div key={i}>
              <div key={`div_position_${i}`}>
                <label>Book Position:</label>
                <input className='position' defaultValue={item.position} onChange={handleSeriesItemPositionChange} type='text' id={`position_${i}`} />
              </div>
              <div key={`div_bookId_${i}`}>
                <label>Book:</label>
                <select className='select-book' defaultValue={item.bookId} onChange={handleSeriesItemBookChange} id={`bookId_${i}`}>
                  <option value='-1'></option>
                  {books.map((book: Book) => (
                      <option key={`${book.id}`} value={`${book.id}`}>{book.title + (book.authors.length > 0 ? ' - ' + book.authors.map(author => author.firstName + ' ' + author.lastName).join(', ') : '')}</option>
                  ))}
                </select>
              </div>
            </div>
          ))}
          <button type='button' onClick={handleAddSeriesItem}>+ Book</button>
        </div>
      }
      {!isEditing &&
        <div>
          <button className='card-button' onClick={() => setIsEditing(true)}>Edit</button>
          <button className='card-button' onClick={() => deleteSeries(series.id)}>Delete</button>
        </div>
      }
      {isEditing &&
        <div>
          <button className='card-button' onClick={() => handleSaveSeries()}>Save</button>
        </div>
      }
    </div>
  )
};

export default ExistingSeries;