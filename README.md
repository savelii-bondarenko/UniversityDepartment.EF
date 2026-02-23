# Electronic Department API

An ASP.NET Core Web API for managing an electronic university department. This system provides role-based access to manage departments, groups, subjects, students, teachers, and grades.

The application is built using **.NET 8** and follows a classic N-Tier (Clean) architecture using **Entity Framework Core 9** for data access.

## 🚀 Technologies Used

* **Framework:** .NET 8, ASP.NET Core Web API
* **Database:** SQL Server, Entity Framework Core 9.0.5
* **Authentication:** JWT (JSON Web Tokens)
* **Mapping:** AutoMapper
* **Testing:** xUnit, Moq, Coverlet
* **Documentation:** Swagger (Swashbuckle)

## 🏗️ Architecture Design

The solution is divided into several loosely coupled projects to separate concerns:

1. **ElectronicDepartment.PL (Presentation Layer):** The ASP.NET Core Web API containing Controllers, Swagger configuration, and JWT authentication middleware.
2. **ElectronicDepartment.BLL (Business Logic Layer):** Contains the business rules, Services, DTOs, and AutoMapper profiles.
3. **ElectronicDepartment.DAL (Data Access Layer):** Manages the database context (`ElectronicDepartmentContext`), Entities, Repositories (Generic and Specific), and the Unit of Work pattern.
4. **ElectronicDepartment.Common:** Shared resources such as Enums (User Roles), custom Exceptions, and configuration settings (`JwtSettings`).
5. **ElectronicDepartment.Tests:** Unit tests covering the BLL services using xUnit and Moq.

## 🔑 Key Features

* **Role-Based Authorization:** Supports four main roles: `Admin`, `Manager`, `Teacher`, and `Student`.
* **JWT Authentication:** Secure token-based authentication.
* **University Management:** CRUD operations for Departments, Groups, Subjects, and Users.
* **Grade Management:** Teachers can assign and delete grades for students on specific subjects.
* **Unit of Work & Repository Pattern:** Clean data access abstraction.
* **Exception Handling:** Custom exceptions (e.g., `EntityNotFoundException`, `ValidationException`) handled gracefully.

## 🛠️ Getting Started

### Prerequisites

* [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
* SQL Server (LocalDB or full instance)

### 1. Database Configuration

Update the connection string in `ElectronicDepartment.PL/appsettings.json` to point to your SQL Server instance.

If you are setting up a local SQL Server using a superuser, you can use the following credentials format:

* **Login:** `sa`
* **Password:** `StrongPassw0rd!`

```json
"ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=ElectronicDepartmentDb;User=sa;Password=StrongPassw0rd!;TrustServerCertificate=True;"
}

```

### 2. Apply Migrations

The project uses Entity Framework Core Code-First migrations. Open a terminal in the solution root and run the following commands to create the database and apply the initial schema:

```bash
dotnet ef migrations add InitialCreate --project ElectronicDepartment.DAL --startup-project ElectronicDepartment.PL
dotnet ef database update --project ElectronicDepartment.DAL --startup-project ElectronicDepartment.PL

```

### 3. Run the Application

Navigate to the Presentation Layer and run the project:

```bash
cd ElectronicDepartment.PL
dotnet run

```

By default, the application will be accessible at `http://localhost:5237` or `https://localhost:7055`.

## 📚 API Documentation

The API is documented using Swagger. Once the application is running, open your browser and navigate to:

```
http://localhost:5237/swagger

```

To test secured endpoints, first authenticate via `POST /api/Auth/login` to receive a token, then click **Authorize** in Swagger and enter `Bearer <your-token>`.

## 🧪 Running Tests

The project includes a comprehensive suite of unit tests for the Business Logic Layer. To execute the tests, run:

```bash
dotnet test

```