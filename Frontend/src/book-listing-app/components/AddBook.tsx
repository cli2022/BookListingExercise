import { Book, BookAuthor } from '@/types/definitions';
import * as React from 'react';

type Props = {
  addBook: (e: React.FormEvent, newBook: Book) => void
};

// Component for adding a new book
const AddBook: React.FC<Props> = ({ addBook }) => {
  const [title, setTitle] = React.useState<string>();
  const [authors, setAuthors] = React.useState<BookAuthor[]>([]);

  const handleTitleChange = (e: React.FormEvent<HTMLInputElement>) => {
    setTitle(e.currentTarget.value);
  };

  const handleAddAuthor = () => {
    const newAuthor = { firstName: '', lastName: '' } as BookAuthor;
    setAuthors([...authors, newAuthor]);
  };

  const handleAuthorChange = (e: React.FormEvent<HTMLInputElement>) => {
    const [field, suffix] = e.currentTarget.id.split('_');
    const authorIndex = Number(suffix);
    if (field === 'firstName') {
      authors[authorIndex].firstName = e.currentTarget.value;
    } else {
      authors[authorIndex].lastName = e.currentTarget.value;
    }
  };

  return (
    <form className='form' onSubmit={(e) => addBook(e, { title: title, authors: authors.filter(author => author.firstName || author.lastName) } as Book)}>
      <div>
        <div className='form-field'>
          <label>Title</label>
          <input onChange={handleTitleChange} type='text' id='title' />
        </div>
        {authors?.map((author: BookAuthor, i: number) => (
          <div key={i}>
            <div className='form-field'>
              <label>Author{authors.length > 1 ? ' ' + (i + 1) : ''} First Name</label>
              <input onChange={handleAuthorChange} type='text' id={`firstName_${i}`} />
            </div>
            <div className='form-field'>
              <label>Author{authors.length > 1 ? ' ' + (i + 1) : ''} Last Name</label>
              <input onChange={handleAuthorChange} type='text' id={`lastName_${i}`} />
            </div>
          </div>
        ))}
      </div>
      <button type='button' onClick={handleAddAuthor}>+ Author</button>
      <button className='form-button' disabled={title === undefined || title.trim() === '' ? true : false}>Add New Book</button>
    </form>
  )
};

export default AddBook;