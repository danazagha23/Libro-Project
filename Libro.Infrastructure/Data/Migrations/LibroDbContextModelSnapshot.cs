﻿// <auto-generated />
using System;
using Libro.Infrastructure.Data.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Libro.Infrastructure.Migrations
{
    [DbContext(typeof(LibroDbContext))]
    partial class LibroDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("BookReadingList", b =>
                {
                    b.Property<int>("BooksBookId")
                        .HasColumnType("int");

                    b.Property<int>("ReadingListsId")
                        .HasColumnType("int");

                    b.HasKey("BooksBookId", "ReadingListsId");

                    b.HasIndex("ReadingListsId");

                    b.ToTable("BookReadingList");
                });

            modelBuilder.Entity("Libro.Domain.Entities.Author", b =>
                {
                    b.Property<int>("AuthorId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AuthorId"));

                    b.Property<string>("AuthorName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Biography")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("AuthorId");

                    b.ToTable("Authors");

                    b.HasData(
                        new
                        {
                            AuthorId = 1,
                            AuthorName = "J.K. Rowling",
                            Biography = "British author, best known for the Harry Potter series."
                        },
                        new
                        {
                            AuthorId = 2,
                            AuthorName = "George R.R. Martin",
                            Biography = "American novelist and short story writer, known for A Song of Ice and Fire series."
                        },
                        new
                        {
                            AuthorId = 3,
                            AuthorName = "Agatha Christie",
                            Biography = "English writer, famous for her detective novels."
                        },
                        new
                        {
                            AuthorId = 4,
                            AuthorName = "Stephen King",
                            Biography = "American author, known for his horror and supernatural fiction."
                        },
                        new
                        {
                            AuthorId = 5,
                            AuthorName = "J.R.R. Tolkien",
                            Biography = "English writer, poet, philologist, and academic, best known for The Hobbit and The Lord of the Rings."
                        },
                        new
                        {
                            AuthorId = 6,
                            AuthorName = "Dan Brown",
                            Biography = "American author, known for his thriller novels including The Da Vinci Code."
                        },
                        new
                        {
                            AuthorId = 7,
                            AuthorName = "Mary Shelley",
                            Biography = "English novelist, short story writer, dramatist, essayist, biographer, and travel writer, best known for her Gothic novel Frankenstein."
                        },
                        new
                        {
                            AuthorId = 8,
                            AuthorName = "Jane Austen",
                            Biography = "English novelist known primarily for her six major novels, including Pride and Prejudice."
                        },
                        new
                        {
                            AuthorId = 9,
                            AuthorName = "Gillian Flynn",
                            Biography = "American author, screenwriter, and former television critic, known for her thriller novels such as Gone Girl."
                        },
                        new
                        {
                            AuthorId = 10,
                            AuthorName = "Harper Lee",
                            Biography = "American novelist, best known for her novel To Kill a Mockingbird."
                        },
                        new
                        {
                            AuthorId = 11,
                            AuthorName = "F. Scott Fitzgerald",
                            Biography = "American novelist and short-story writer, famous for his novel The Great Gatsby."
                        },
                        new
                        {
                            AuthorId = 12,
                            AuthorName = "J.D. Salinger",
                            Biography = "American writer, best known for his novel The Catcher in the Rye."
                        },
                        new
                        {
                            AuthorId = 13,
                            AuthorName = "Suzanne Collins",
                            Biography = "American television writer and novelist, known for The Hunger Games trilogy."
                        },
                        new
                        {
                            AuthorId = 14,
                            AuthorName = "George Orwell",
                            Biography = "English novelist, essayist, journalist, and critic, known for his dystopian novel 1984."
                        },
                        new
                        {
                            AuthorId = 15,
                            AuthorName = "Charlotte Brontë",
                            Biography = "English novelist and poet, best known for her novel Jane Eyre."
                        },
                        new
                        {
                            AuthorId = 16,
                            AuthorName = "C.S. Lewis",
                            Biography = "British writer and theologian, known for The Chronicles of Narnia series."
                        },
                        new
                        {
                            AuthorId = 17,
                            AuthorName = "John Green",
                            Biography = "American author and YouTube content creator, known for his young adult romance novels."
                        },
                        new
                        {
                            AuthorId = 18,
                            AuthorName = "Aldous Huxley",
                            Biography = "English writer and philosopher, best known for his dystopian novel Brave New World."
                        },
                        new
                        {
                            AuthorId = 19,
                            AuthorName = "Paulo Coelho",
                            Biography = "Brazilian lyricist and novelist, known for his novel The Alchemist."
                        });
                });

            modelBuilder.Entity("Libro.Domain.Entities.Book", b =>
                {
                    b.Property<int>("BookId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("BookId"));

                    b.Property<int>("AvailabilityStatus")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("GenreId")
                        .HasColumnType("int");

                    b.Property<DateTime>("PublicationDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("BookId");

                    b.HasIndex("GenreId");

                    b.ToTable("Books");

                    b.HasData(
                        new
                        {
                            BookId = 1,
                            AvailabilityStatus = 0,
                            Description = "The first book in the Harry Potter series.",
                            GenreId = 1,
                            PublicationDate = new DateTime(1997, 6, 26, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Title = "Harry Potter and the Sorcerer's Stone"
                        },
                        new
                        {
                            BookId = 2,
                            AvailabilityStatus = 0,
                            Description = "The first book in the A Song of Ice and Fire series.",
                            GenreId = 1,
                            PublicationDate = new DateTime(1996, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Title = "A Game of Thrones"
                        },
                        new
                        {
                            BookId = 3,
                            AvailabilityStatus = 0,
                            Description = "A classic detective novel featuring Hercule Poirot.",
                            GenreId = 2,
                            PublicationDate = new DateTime(1934, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Title = "Murder on the Orient Express"
                        },
                        new
                        {
                            BookId = 4,
                            AvailabilityStatus = 0,
                            Description = "A horror novel set in an isolated hotel.",
                            GenreId = 3,
                            PublicationDate = new DateTime(1977, 1, 28, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Title = "The Shining"
                        },
                        new
                        {
                            BookId = 5,
                            AvailabilityStatus = 0,
                            Description = "A fantasy novel by J.R.R. Tolkien.",
                            GenreId = 1,
                            PublicationDate = new DateTime(1937, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Title = "The Hobbit"
                        },
                        new
                        {
                            BookId = 6,
                            AvailabilityStatus = 0,
                            Description = "A mystery thriller novel by Dan Brown.",
                            GenreId = 2,
                            PublicationDate = new DateTime(2003, 3, 18, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Title = "The Da Vinci Code"
                        },
                        new
                        {
                            BookId = 7,
                            AvailabilityStatus = 0,
                            Description = "A science fiction novel by Mary Shelley.",
                            GenreId = 4,
                            PublicationDate = new DateTime(1818, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Title = "Frankenstein"
                        },
                        new
                        {
                            BookId = 8,
                            AvailabilityStatus = 0,
                            Description = "A classic romance novel by Jane Austen.",
                            GenreId = 5,
                            PublicationDate = new DateTime(1813, 1, 28, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Title = "Pride and Prejudice"
                        },
                        new
                        {
                            BookId = 9,
                            AvailabilityStatus = 0,
                            Description = "A psychological thriller novel by Gillian Flynn.",
                            GenreId = 6,
                            PublicationDate = new DateTime(2012, 6, 5, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Title = "Gone Girl"
                        },
                        new
                        {
                            BookId = 10,
                            AvailabilityStatus = 0,
                            Description = "A coming-of-age novel by Harper Lee.",
                            GenreId = 7,
                            PublicationDate = new DateTime(1960, 7, 11, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Title = "To Kill a Mockingbird"
                        },
                        new
                        {
                            BookId = 11,
                            AvailabilityStatus = 0,
                            Description = "A classic novel by F. Scott Fitzgerald.",
                            GenreId = 5,
                            PublicationDate = new DateTime(1925, 4, 10, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Title = "The Great Gatsby"
                        },
                        new
                        {
                            BookId = 12,
                            AvailabilityStatus = 0,
                            Description = "A novel by J.D. Salinger.",
                            GenreId = 7,
                            PublicationDate = new DateTime(1951, 7, 16, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Title = "The Catcher in the Rye"
                        },
                        new
                        {
                            BookId = 13,
                            AvailabilityStatus = 0,
                            Description = "The first book in The Hunger Games trilogy by Suzanne Collins.",
                            GenreId = 1,
                            PublicationDate = new DateTime(2008, 9, 14, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Title = "The Hunger Games"
                        },
                        new
                        {
                            BookId = 14,
                            AvailabilityStatus = 0,
                            Description = "A dystopian novel by George Orwell.",
                            GenreId = 4,
                            PublicationDate = new DateTime(1949, 6, 8, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Title = "1984"
                        },
                        new
                        {
                            BookId = 15,
                            AvailabilityStatus = 0,
                            Description = "A novel by Charlotte Brontë.",
                            GenreId = 5,
                            PublicationDate = new DateTime(1847, 10, 16, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Title = "Jane Eyre"
                        },
                        new
                        {
                            BookId = 16,
                            AvailabilityStatus = 0,
                            Description = "A series of fantasy novels by C.S. Lewis.",
                            GenreId = 1,
                            PublicationDate = new DateTime(1950, 10, 16, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Title = "The Chronicles of Narnia"
                        },
                        new
                        {
                            BookId = 17,
                            AvailabilityStatus = 0,
                            Description = "A young adult romance novel by John Green.",
                            GenreId = 5,
                            PublicationDate = new DateTime(2012, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Title = "The Fault in Our Stars"
                        },
                        new
                        {
                            BookId = 18,
                            AvailabilityStatus = 0,
                            Description = "A dystopian novel by Aldous Huxley.",
                            GenreId = 4,
                            PublicationDate = new DateTime(1932, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Title = "Brave New World"
                        },
                        new
                        {
                            BookId = 19,
                            AvailabilityStatus = 0,
                            Description = "A novel by Paulo Coelho.",
                            GenreId = 1,
                            PublicationDate = new DateTime(1988, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Title = "The Alchemist"
                        });
                });

            modelBuilder.Entity("Libro.Domain.Entities.BookAuthor", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("AuthorId")
                        .HasColumnType("int");

                    b.Property<int>("BookId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("BookId");

                    b.ToTable("BookAuthors");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            AuthorId = 1,
                            BookId = 1
                        },
                        new
                        {
                            Id = 2,
                            AuthorId = 2,
                            BookId = 2
                        },
                        new
                        {
                            Id = 3,
                            AuthorId = 3,
                            BookId = 3
                        },
                        new
                        {
                            Id = 4,
                            AuthorId = 4,
                            BookId = 4
                        },
                        new
                        {
                            Id = 5,
                            AuthorId = 5,
                            BookId = 5
                        },
                        new
                        {
                            Id = 6,
                            AuthorId = 6,
                            BookId = 6
                        },
                        new
                        {
                            Id = 7,
                            AuthorId = 7,
                            BookId = 7
                        },
                        new
                        {
                            Id = 8,
                            AuthorId = 8,
                            BookId = 8
                        },
                        new
                        {
                            Id = 9,
                            AuthorId = 9,
                            BookId = 9
                        },
                        new
                        {
                            Id = 10,
                            AuthorId = 10,
                            BookId = 10
                        },
                        new
                        {
                            Id = 11,
                            AuthorId = 11,
                            BookId = 11
                        },
                        new
                        {
                            Id = 12,
                            AuthorId = 12,
                            BookId = 12
                        },
                        new
                        {
                            Id = 13,
                            AuthorId = 13,
                            BookId = 13
                        },
                        new
                        {
                            Id = 14,
                            AuthorId = 14,
                            BookId = 14
                        },
                        new
                        {
                            Id = 15,
                            AuthorId = 15,
                            BookId = 15
                        },
                        new
                        {
                            Id = 16,
                            AuthorId = 16,
                            BookId = 16
                        },
                        new
                        {
                            Id = 17,
                            AuthorId = 17,
                            BookId = 17
                        },
                        new
                        {
                            Id = 18,
                            AuthorId = 18,
                            BookId = 18
                        },
                        new
                        {
                            Id = 19,
                            AuthorId = 19,
                            BookId = 19
                        });
                });

            modelBuilder.Entity("Libro.Domain.Entities.BookTransaction", b =>
                {
                    b.Property<int>("TransactionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("TransactionId"));

                    b.Property<int>("BookId")
                        .HasColumnType("int");

                    b.Property<DateTime>("DueDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsReturned")
                        .HasColumnType("bit");

                    b.Property<int>("PatronId")
                        .HasColumnType("int");

                    b.Property<DateTime>("TransactionDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("TransactionType")
                        .HasColumnType("int");

                    b.HasKey("TransactionId");

                    b.HasIndex("BookId");

                    b.HasIndex("PatronId");

                    b.ToTable("BookTransactions");
                });

            modelBuilder.Entity("Libro.Domain.Entities.Genre", b =>
                {
                    b.Property<int>("GenreId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("GenreId"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("GenreId");

                    b.ToTable("Genres");

                    b.HasData(
                        new
                        {
                            GenreId = 1,
                            Name = "Fantasy"
                        },
                        new
                        {
                            GenreId = 2,
                            Name = "Mystery"
                        },
                        new
                        {
                            GenreId = 3,
                            Name = "Horror"
                        },
                        new
                        {
                            GenreId = 4,
                            Name = "Science Fiction"
                        },
                        new
                        {
                            GenreId = 5,
                            Name = "Romance"
                        },
                        new
                        {
                            GenreId = 6,
                            Name = "Thriller"
                        },
                        new
                        {
                            GenreId = 7,
                            Name = "Historical Fiction"
                        });
                });

            modelBuilder.Entity("Libro.Domain.Entities.ReadingList", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("ReadingLists");
                });

            modelBuilder.Entity("Libro.Domain.Entities.Review", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("BookId")
                        .HasColumnType("int");

                    b.Property<string>("Comment")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Rating")
                        .HasColumnType("int");

                    b.Property<DateTime>("ReviewDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("BookId");

                    b.HasIndex("UserId");

                    b.ToTable("Reviews");
                });

            modelBuilder.Entity("Libro.Domain.Entities.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserId"));

                    b.Property<string>("Address")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Role")
                        .HasColumnType("int");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("BookReadingList", b =>
                {
                    b.HasOne("Libro.Domain.Entities.Book", null)
                        .WithMany()
                        .HasForeignKey("BooksBookId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Libro.Domain.Entities.ReadingList", null)
                        .WithMany()
                        .HasForeignKey("ReadingListsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Libro.Domain.Entities.Book", b =>
                {
                    b.HasOne("Libro.Domain.Entities.Genre", "Genre")
                        .WithMany("Books")
                        .HasForeignKey("GenreId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Genre");
                });

            modelBuilder.Entity("Libro.Domain.Entities.BookAuthor", b =>
                {
                    b.HasOne("Libro.Domain.Entities.Author", "Author")
                        .WithMany("BookAuthors")
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Libro.Domain.Entities.Book", "Book")
                        .WithMany("BookAuthors")
                        .HasForeignKey("BookId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");

                    b.Navigation("Book");
                });

            modelBuilder.Entity("Libro.Domain.Entities.BookTransaction", b =>
                {
                    b.HasOne("Libro.Domain.Entities.Book", "Book")
                        .WithMany()
                        .HasForeignKey("BookId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Libro.Domain.Entities.User", "Patron")
                        .WithMany("Transactions")
                        .HasForeignKey("PatronId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Book");

                    b.Navigation("Patron");
                });

            modelBuilder.Entity("Libro.Domain.Entities.ReadingList", b =>
                {
                    b.HasOne("Libro.Domain.Entities.User", "User")
                        .WithMany("ReadingLists")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Libro.Domain.Entities.Review", b =>
                {
                    b.HasOne("Libro.Domain.Entities.Book", "Book")
                        .WithMany("Reviews")
                        .HasForeignKey("BookId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Libro.Domain.Entities.User", "User")
                        .WithMany("Reviews")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Book");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Libro.Domain.Entities.Author", b =>
                {
                    b.Navigation("BookAuthors");
                });

            modelBuilder.Entity("Libro.Domain.Entities.Book", b =>
                {
                    b.Navigation("BookAuthors");

                    b.Navigation("Reviews");
                });

            modelBuilder.Entity("Libro.Domain.Entities.Genre", b =>
                {
                    b.Navigation("Books");
                });

            modelBuilder.Entity("Libro.Domain.Entities.User", b =>
                {
                    b.Navigation("ReadingLists");

                    b.Navigation("Reviews");

                    b.Navigation("Transactions");
                });
#pragma warning restore 612, 618
        }
    }
}
