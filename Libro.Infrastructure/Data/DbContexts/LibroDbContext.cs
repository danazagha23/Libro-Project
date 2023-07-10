using Libro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Infrastructure.Data.DbContexts
{
    public class LibroDbContext : DbContext
    {
        public LibroDbContext(DbContextOptions<LibroDbContext> options) : base(options)
        {

        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<BookAuthor> BookAuthors { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<BookTransaction> BookTransactions { get; set; }
        public DbSet<ReadingList> ReadingLists { get; set; }
        public DbSet<Review> Reviews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Seeding Genres
            modelBuilder.Entity<Genre>().HasData(
                new Genre { GenreId = 1, Name = "Fantasy" },
                new Genre { GenreId = 2, Name = "Mystery" },
                new Genre { GenreId = 3, Name = "Horror" },
                new Genre { GenreId = 4, Name = "Science Fiction" },
                new Genre { GenreId = 5, Name = "Romance" },
                new Genre { GenreId = 6, Name = "Thriller" },
                new Genre { GenreId = 7, Name = "Historical Fiction" }
            );

            // Seeding Books
            modelBuilder.Entity<Book>().HasData(
                new Book
                {
                    BookId = 1,
                    Title = "Harry Potter and the Sorcerer's Stone",
                    Description = "The first book in the Harry Potter series.",
                    GenreId = 1,
                    PublicationDate = new DateTime(1997, 6, 26)
                },
                new Book
                {
                    BookId = 2,
                    Title = "A Game of Thrones",
                    Description = "The first book in the A Song of Ice and Fire series.",
                    GenreId = 1,
                    PublicationDate = new DateTime(1996, 8, 1)
                },
                new Book
                {
                    BookId = 3,
                    Title = "Murder on the Orient Express",
                    Description = "A classic detective novel featuring Hercule Poirot.",
                    GenreId = 2,
                    PublicationDate = new DateTime(1934, 1, 1)
                },
                new Book
                {
                    BookId = 4,
                    Title = "The Shining",
                    Description = "A horror novel set in an isolated hotel.",
                    GenreId = 3,
                    PublicationDate = new DateTime(1977, 1, 28)
                },
                new Book
                {
                    BookId = 5,
                    Title = "The Hobbit",
                    Description = "A fantasy novel by J.R.R. Tolkien.",
                    GenreId = 1,
                    PublicationDate = new DateTime(1937, 9, 21)
                },
                new Book
                {
                    BookId = 6,
                    Title = "The Da Vinci Code",
                    Description = "A mystery thriller novel by Dan Brown.",
                    GenreId = 2,
                    PublicationDate = new DateTime(2003, 3, 18)
                },
                new Book
                {
                    BookId = 7,
                    Title = "Frankenstein",
                    Description = "A science fiction novel by Mary Shelley.",
                    GenreId = 4,
                    PublicationDate = new DateTime(1818, 1, 1)
                },
                new Book
                {
                    BookId = 8,
                    Title = "Pride and Prejudice",
                    Description = "A classic romance novel by Jane Austen.",
                    GenreId = 5,
                    PublicationDate = new DateTime(1813, 1, 28)
                },
                new Book
                {
                    BookId = 9,
                    Title = "Gone Girl",
                    Description = "A psychological thriller novel by Gillian Flynn.",
                    GenreId = 6,
                    PublicationDate = new DateTime(2012, 6, 5)
                },
                new Book
                {
                    BookId = 10,
                    Title = "To Kill a Mockingbird",
                    Description = "A coming-of-age novel by Harper Lee.",
                    GenreId = 7,
                    PublicationDate = new DateTime(1960, 7, 11)
                },
                new Book
                {
                    BookId = 11,
                    Title = "The Great Gatsby",
                    Description = "A classic novel by F. Scott Fitzgerald.",
                    GenreId = 5,
                    PublicationDate = new DateTime(1925, 4, 10)
                },
                new Book
                {
                    BookId = 12,
                    Title = "The Catcher in the Rye",
                    Description = "A novel by J.D. Salinger.",
                    GenreId = 7,
                    PublicationDate = new DateTime(1951, 7, 16)
                },
                new Book
                {
                    BookId = 13,
                    Title = "The Hunger Games",
                    Description = "The first book in The Hunger Games trilogy by Suzanne Collins.",
                    GenreId = 1,
                    PublicationDate = new DateTime(2008, 9, 14)
                },
                new Book
                {
                    BookId = 14,
                    Title = "1984",
                    Description = "A dystopian novel by George Orwell.",
                    GenreId = 4,
                    PublicationDate = new DateTime(1949, 6, 8)
                },
                new Book
                {
                    BookId = 15,
                    Title = "Jane Eyre",
                    Description = "A novel by Charlotte Brontë.",
                    GenreId = 5,
                    PublicationDate = new DateTime(1847, 10, 16)
                },
                new Book
                {
                    BookId = 16,
                    Title = "The Chronicles of Narnia",
                    Description = "A series of fantasy novels by C.S. Lewis.",
                    GenreId = 1,
                    PublicationDate = new DateTime(1950, 10, 16)
                },
                new Book
                {
                    BookId = 17,
                    Title = "The Fault in Our Stars",
                    Description = "A young adult romance novel by John Green.",
                    GenreId = 5,
                    PublicationDate = new DateTime(2012, 1, 10)
                },
                new Book
                {
                    BookId = 18,
                    Title = "Brave New World",
                    Description = "A dystopian novel by Aldous Huxley.",
                    GenreId = 4,
                    PublicationDate = new DateTime(1932, 1, 1)
                },
                new Book
                {
                    BookId = 19,
                    Title = "The Alchemist",
                    Description = "A novel by Paulo Coelho.",
                    GenreId = 1,
                    PublicationDate = new DateTime(1988, 1, 1)
                }
            );

            // Seeding Authors
            modelBuilder.Entity<Author>().HasData(
                new Author
                {
                    AuthorId = 1,
                    AuthorName = "J.K. Rowling",
                    Biography = "British author, best known for the Harry Potter series."
                },
                new Author
                {
                    AuthorId = 2,
                    AuthorName = "George R.R. Martin",
                    Biography = "American novelist and short story writer, known for A Song of Ice and Fire series."
                },
                new Author
                {
                    AuthorId = 3,
                    AuthorName = "Agatha Christie",
                    Biography = "English writer, famous for her detective novels."
                },
                new Author
                {
                    AuthorId = 4,
                    AuthorName = "Stephen King",
                    Biography = "American author, known for his horror and supernatural fiction."
                },
                new Author
                {
                    AuthorId = 5,
                    AuthorName = "J.R.R. Tolkien",
                    Biography = "English writer, poet, philologist, and academic, best known for The Hobbit and The Lord of the Rings."
                },
                new Author
                {
                    AuthorId = 6,
                    AuthorName = "Dan Brown",
                    Biography = "American author, known for his thriller novels including The Da Vinci Code."
                },
                new Author
                {
                    AuthorId = 7,
                    AuthorName = "Mary Shelley",
                    Biography = "English novelist, short story writer, dramatist, essayist, biographer, and travel writer, best known for her Gothic novel Frankenstein."
                },
                new Author
                {
                    AuthorId = 8,
                    AuthorName = "Jane Austen",
                    Biography = "English novelist known primarily for her six major novels, including Pride and Prejudice."
                },
                new Author
                {
                    AuthorId = 9,
                    AuthorName = "Gillian Flynn",
                    Biography = "American author, screenwriter, and former television critic, known for her thriller novels such as Gone Girl."
                },
                new Author
                {
                    AuthorId = 10,
                    AuthorName = "Harper Lee",
                    Biography = "American novelist, best known for her novel To Kill a Mockingbird."
                },
                new Author
                {
                    AuthorId = 11,
                    AuthorName = "F. Scott Fitzgerald",
                    Biography = "American novelist and short-story writer, famous for his novel The Great Gatsby."
                },
                new Author
                {
                    AuthorId = 12,
                    AuthorName = "J.D. Salinger",
                    Biography = "American writer, best known for his novel The Catcher in the Rye."
                },
                new Author
                {
                    AuthorId = 13,
                    AuthorName = "Suzanne Collins",
                    Biography = "American television writer and novelist, known for The Hunger Games trilogy."
                },
                new Author
                {
                    AuthorId = 14,
                    AuthorName = "George Orwell",
                    Biography = "English novelist, essayist, journalist, and critic, known for his dystopian novel 1984."
                },
                new Author
                {
                    AuthorId = 15,
                    AuthorName = "Charlotte Brontë",
                    Biography = "English novelist and poet, best known for her novel Jane Eyre."
                },
                new Author
                {
                    AuthorId = 16,
                    AuthorName = "C.S. Lewis",
                    Biography = "British writer and theologian, known for The Chronicles of Narnia series."
                },
                new Author
                {
                    AuthorId = 17,
                    AuthorName = "John Green",
                    Biography = "American author and YouTube content creator, known for his young adult romance novels."
                },
                new Author
                {
                    AuthorId = 18,
                    AuthorName = "Aldous Huxley",
                    Biography = "English writer and philosopher, best known for his dystopian novel Brave New World."
                },
                new Author
                {
                    AuthorId = 19,
                    AuthorName = "Paulo Coelho",
                    Biography = "Brazilian lyricist and novelist, known for his novel The Alchemist."
                }
            );

            // Seeding BookAuthors
            modelBuilder.Entity<BookAuthor>().HasData(
                new BookAuthor
                {
                    Id = 1,
                    AuthorId = 1,
                    BookId = 1
                },
                new BookAuthor
                {
                    Id = 2,
                    AuthorId = 2,
                    BookId = 2
                },
                new BookAuthor
                {
                    Id = 3,
                    AuthorId = 3,
                    BookId = 3
                },
                new BookAuthor
                {
                    Id = 4,
                    AuthorId = 4,
                    BookId = 4
                },
                new BookAuthor
                {
                    Id = 5,
                    AuthorId = 5,
                    BookId = 5
                },
                new BookAuthor
                {
                    Id = 6,
                    AuthorId = 6,
                    BookId = 6
                },
                new BookAuthor
                {
                    Id = 7,
                    AuthorId = 7,
                    BookId = 7
                },
                new BookAuthor
                {
                    Id = 8,
                    AuthorId = 8,
                    BookId = 8
                },  
                new BookAuthor
                {
                    Id = 9,
                    AuthorId = 9,
                    BookId = 9
                },
                new BookAuthor
                {
                    Id = 10,
                    AuthorId = 10,
                    BookId = 10
                },
                new BookAuthor
                {
                    Id = 11,
                    AuthorId = 11,
                    BookId = 11
                },
                new BookAuthor
                {
                    Id = 12,
                    AuthorId = 12,
                    BookId = 12
                },
                new BookAuthor
                {
                    Id = 13,
                    AuthorId = 13,
                    BookId = 13
                },
                new BookAuthor
                {
                    Id = 14,
                    AuthorId = 14,
                    BookId = 14
                },
                new BookAuthor
                {
                    Id = 15,
                    AuthorId = 15,
                    BookId = 15
                },
                new BookAuthor
                {
                    Id = 16,
                    AuthorId = 16,
                    BookId = 16
                },
                new BookAuthor
                {
                    Id = 17,
                    AuthorId = 17,
                    BookId = 17
                },
                new BookAuthor
                {
                    Id = 18,
                    AuthorId = 18,
                    BookId = 18
                },
                new BookAuthor
                {
                    Id = 19,
                    AuthorId = 19,
                    BookId = 19
                }
            );
        }
    }
}
