# federicopessina-Library

The **federicopessina-Library** project is a comprehensive library management system that enables efficient management of books, users, reservations, and publications. It is designed with a modular architecture that separates concerns into distinct layers, including core business logic, APIs, user interfaces, and testing frameworks. This structure ensures scalability, maintainability, and ease of integration with various external systems.

## Key Features

### 1. **Core Library System**
At the heart of the system is the core business logic that handles the essential operations of a library. This includes managing entities such as books, users, reservations, and publications. The core system is responsible for implementing the business rules, exception handling, and data persistence.

### 2. **User Management**
The system includes user registration and management features, allowing users to create accounts, update their information, and interact with the library system. This functionality is exposed through API controllers and a console interface, providing flexibility for different use cases.

### 3. **Reservation System**
The reservation system allows users to reserve books and publications. It ensures that only available items can be reserved and manages the reservation lifecycle, including the creation, updating, and cancellation of reservations.

### 4. **Data Persistence**
The library system uses a combination of in-memory stores and database-backed stores (via Entity Framework) for managing data. The data access layer includes stores for each entity type, such as `BookStore`, `UserStore`, `ReservationStore`, etc., which handle CRUD operations and data validation.

### 5. **API and Console Interfaces**
The system provides multiple ways to interact with the library:
- **API**: A RESTful API that exposes all core functionalities, enabling integration with other systems or providing an interface for front-end applications.
- **Console Application**: A simple command-line interface for interacting with the library system, useful for administrative tasks or testing.

### 6. **Testing**
Unit tests are integrated across different layers of the system to ensure the correctness of business logic, data access, and API endpoints. The tests cover various components, such as entity models, store operations, and controller actions.

## Architecture

The project is organized into several key modules, each serving a distinct purpose:

- **Entities**: Contains the core domain models (e.g., `Book`, `User`, `Reservation`, etc.), representing the data structure of the system.
- **Interfaces**: Defines the contracts for the services and stores. These interfaces are implemented by the core logic and the API.
- **Library**: Implements the core business logic, including services, stores, and exception handling.
- **Library.Console**: A console application that provides a text-based interface for interacting with the library system.
- **Library.Core.API**: A RESTful API that exposes the core functionality of the library over HTTP, allowing external systems to interact with it.
- **WebApplication1**: A basic web application that serves as a sample front-end for interacting with the library system.
- **Tests**: Contains unit tests for different layers of the system, including entity models, stores, and API controllers.

## Getting Started

To set up the project locally, follow these steps:

### Prerequisites

- .NET SDK (version 5.0 or later)
- Visual Studio or any other .NET-compatible IDE

### Installation

1. Clone the repository:
   bash
   git clone https://github.com/yourusername/federicopessina-Library.git
2. Navigate to the project directory:
  cd federicopessina-Library
3. Open the solution file in Visual Studio:
  Library.sln
4. Restore the NuGet packages:
  dotnet restore
5. Build the solution:
  dotnet build

### Running the Application
- Console Application: To interact with the library system via the command line, run the following
  dotnet run --project Library.Console/Library.Console.csproj
- API: To run the API and expose the library functionality over HTTP, use the following command:
  dotnet run --project Library.Core.API/Library.Core.API.csproj
- Web Application: To run the web application, use the following:
  dotnet run --project WebApplication1/WebApplication1.csproj
- Running Tests: To run the unit tests and ensure the correctness of the system, execute the following command:
  dotnet test
