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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Seeding Authors
            modelBuilder.Entity<Author>().HasData(
                new Author { AuthorId = 1, AuthorName = "J.K. Rowling", Biography = "British author, best known for the Harry Potter series." },
                new Author { AuthorId = 2, AuthorName = "George R.R. Martin", Biography = "American novelist and short story writer, known for A Song of Ice and Fire series." },
                new Author { AuthorId = 3, AuthorName = "Agatha Christie", Biography = "English writer, famous for her detective novels." },
                new Author { AuthorId = 4, AuthorName = "Stephen King", Biography = "American author, known for his horror and supernatural fiction." }
            );

            // Seeding Genres
            modelBuilder.Entity<Genre>().HasData(
                new Genre { GenreId = 1, Name = "Fantasy" },
                new Genre { GenreId = 2, Name = "Mystery" },
                new Genre { GenreId = 3, Name = "Horror" }
            );

            // Seeding Books
            modelBuilder.Entity<Book>().HasData(
                new Book
                {
                    BookId = 1,
                    Title = "Harry Potter and the Sorcerer's Stone",
                    Description = "The first book in the Harry Potter series.",
                    GenreId = 1,
                    AuthorId = 1,
                    PublicationDate = new DateTime(1997, 6, 26)
                },
                new Book
                {
                    BookId = 2,
                    Title = "A Game of Thrones",
                    Description = "The first book in the A Song of Ice and Fire series.",
                    GenreId = 1,
                    AuthorId = 2,
                    PublicationDate = new DateTime(1996, 8, 1)
                },
                new Book
                {
                    BookId = 3,
                    Title = "Murder on the Orient Express",
                    Description = "A classic detective novel featuring Hercule Poirot.",
                    GenreId = 2,
                    AuthorId = 3,
                    PublicationDate = new DateTime(1934, 1, 1)
                },
                new Book
                {
                    BookId = 4,
                    Title = "The Shining",
                    Description = "A horror novel set in an isolated hotel.",
                    GenreId = 3,
                    AuthorId = 4,
                    PublicationDate = new DateTime(1977, 1, 28)
                }
            );
        }

    }
}
