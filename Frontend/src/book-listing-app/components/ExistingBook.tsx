import { Book, BookAuthor } from '@/types/definitions';
import * as React from 'react';

type Props = {
  book: Book,
  deleteBook: (id: number) => void,
  saveBook: (book: Book) => void
};

// Component for rendering and editing an existing book
const ExistingBook: React.FC<Props> = ({ book, deleteBook, saveBook }) => {
  const [isEditing, setIsEditing] = React.useState<boolean>(false);
  const [authors, setAuthors] = React.useState<BookAuthor[]>(book.authors);

  const handleTitleChange = (e: React.FormEvent<HTMLInputElement>) => {
    book.title = e.currentTarget.value;
  };

  const handleAuthorChange = (e: React.FormEvent<HTMLInputElement>) => {
    const [field, suffix] = e.currentTarget.id.split('_');
    const authorIndex = Number(suffix);
    if (field === 'firstName') {
      authors[authorIndex].firstName = e.currentTarget.value;
    } else {
      authors[authorIndex].lastName = e.currentTarget.value;
    }
    book.authors = authors;
  };

  const handleAddAuthor = () => {
    const newAuthor = { firstName: '', lastName: '' } as BookAuthor;
    setAuthors([...authors, newAuthor]);
    book.authors = authors;
  };

  const handleSaveBook = () => {
    book.authors = book.authors.filter(author => author.firstName || author.lastName).sort((a, b) => (a.firstName + ' ' + a.lastName).localeCompare(b.firstName + ' ' + b.lastName));
    saveBook(book);
    setAuthors(book.authors);
    setIsEditing(false);
  };

  return (
    <div className='card'>
      {!isEditing &&
        <div className='card-body'>
          <h1 className='card-body-title'>{book.title}</h1>
          {book.authors?.length > 0 &&
          <p className='card-body-text'>by {book.authors.map(author => author.firstName + ' ' + author.lastName).join(', ')}</p>}
        </div>
      }
      {isEditing &&
        <div className='card-edit-body'>
          <div>
            <label>Title</label>
            <input defaultValue={book.title} onChange={handleTitleChange} type='text' id='title' />
          </div>
          {authors?.map((author: BookAuthor, i: number) => (
            <div key={i}>
              <label>Author{authors.length > 1 ? ' ' + (i + 1) : ''} First Name</label>
              <input defaultValue={authors[i].firstName} onChange={handleAuthorChange} type='text' id={`firstName_${i}`} />
              <label>Author{authors.length > 1 ? ' ' + (i + 1) : ''} Last Name</label>
              <input defaultValue={authors[i].lastName} onChange={handleAuthorChange} type='text' id={`lastName_${i}`} />
            </div>
          ))}
          <button type='button' onClick={handleAddAuthor}>+ Author</button>
        </div>
      }
      {!isEditing &&
        <div>
          <button className='card-button' onClick={() => setIsEditing(true)}>Edit</button>
          <button className='card-button' onClick={() => deleteBook(book.id)}>Delete</button>
        </div>
      }
      {isEditing &&
        <div>
          <button className='card-button' onClick={() => handleSaveBook()}>Save</button>
        </div>
      }
    </div>
  )
};

export default ExistingBook;