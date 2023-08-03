## Project Description

Libro is a comprehensive Book Management System designed to facilitate the easy management and discovery of books in a library setting. The primary focus of this project is to design and implement the web APIs that will support the functionality of this application. These APIs will handle user registration and authentication, book transactions, patron profile management, book and author management, and more. This project follows the Clean Architecture principles and is built with ASP.NET MVC.

## Main Features

- **User Registration and Authentication:** Users can register and log in to the system. Different types of users (patrons, librarians, administrators) have different access levels.

- **Searching and Browsing Books:** Users can search for books by title, author, genre, and other criteria. Book information pages include details such as title, author, publication date, genre, and availability status.

- **Book Transactions:** Patrons can reserve available books, and librarians can check out and accept returned books. The system tracks due dates and overdue books.

- **Patron Profiles:** Patrons can view their profiles, including borrowing history and current or overdue loans. Librarians and administrators can view and edit patron profiles.

- **Book and Author Management:** Librarians and administrators can add, edit, or remove books and authors in the system.

## Additional Features (Optional)

- **Reading Lists:** Patrons can create and manage custom reading lists.

- **Book Reviews and Ratings:** Patrons can rate and review books, visible to other users.

- **Notifications:** The system sends notifications to patrons about due dates, reserved books, or other events.

- **Book Recommendations:** The system provides personalized book recommendations based on patron's history or favorite genres.


## Technical Requirements

1. **Clean Architecture:** The project follows a clean architecture pattern with clearly defined layers: domain, application, infrastructure, and presentation. This architectural approach promotes code modularity and separation of concerns.

2. **Repository Pattern:** Data access is abstracted using repository interfaces, enhancing the maintainability and testability of the data layer.

3. **JWT Authentication:** Secure user authentication is implemented using JSON Web Tokens (JWT). This ensures safe and efficient user access to the system.

4. **Pagination:** To enhance user experience and optimize performance, search results and book listings are paginated.

5. **Error Handling and Logging:** The system includes robust error handling mechanisms and logging functionality. This aids in debugging and maintaining the application.

6. **Unit Testing (xUnit):** I employ xUnit, a testing framework, for writing and running unit tests. This ensures the reliability and stability of the codebase.

7. **Dependency Injection:** I utilize dependency injection to manage the flow of dependencies in the application. This promotes loose coupling and increases the flexibility of the system.

8. **Secure API Design:** The ASP.NET Core Web API follows best practices for secure API design, such as input validation and authorization checks, and output sanitization.

9. **Database Migrations:** I use Entity Framework Core's migration feature to manage database schema changes. This simplifies database versioning and deployment.

## Front-end: Razor Pages

The front-end of the Libro Library Management System is developed using Razor Pages, a lightweight web framework for building web applications with ASP.NET Core. Razor Pages allow for efficient and maintainable UI development, and they seamlessly integrate with the back-end APIs.

## Project Management

The project is managed using Jira to track progress and manage tasks effectively.

## Getting Started

To run the Libro Library Management System locally, follow these steps:

1. Clone the repository: `git clone https://github.com/danazagha23/Libro-Project.git`
2. Navigate to the project folder: `cd Libro-Project`
3. Set up the database and data migration.
4. Install dependencies: `dotnet restore`
5. Build the project: `dotnet build`
6. Run the application: `dotnet run`
7. Access the application in your web browser at `http://localhost:5228` or `https://localhost:7292`

## Technologies and Frameworks Used

- C# .NET Core
- Entity Framework Core
- ASP.NET Core Web API
- Razor Pages for Front-end
- JWT Authentication
- Bootstrap for styling
- MVC (Model-View-Controller) for the Presentation Layer
- MailKit for sending emails
- xUnit for Testing

