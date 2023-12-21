// Type definitions

export type Book = {
    id: number;
    title: string;
    authors: BookAuthor[];
};

export type BookAuthor = {
    firstName: string;
    lastName: string;
};

export type BookSeries = {
    id: number;
    name: string;
    books: BookSeriesItem[];
};

export type BookSeriesItem = {
    bookId: number;
    position: number;
};