# ExpenseSharingApplication

ExpenseSharingApplication is a web application designed to manage and share expenses among users. The backend is built with .NET using Entity Framework Core for database operations, and the frontend is developed using Angular. The application includes predefined users for demonstration purposes.

## Features

- **User Authentication and Authorization**
- **Admin and Normal User Roles**
- **Expense Management**
- **Expense Sharing and Settlement**
- **User Management (Admins only)**

## Technology Stack

- **Backend**: .NET Core with Entity Framework Core
- **Frontend**: Angular
- **Database**: SQL Server (or specify if different)

## Seeded Users

The application comes with six seeded users for testing:

### Admin Users:

- **Admin1**
  - Username: `admin1@gmail.com`
  - Password: `Admin@123`
- **Admin2**
  - Username: `admin2@gmail.com`
  - Password: `Admin@123`

### Normal Users:

- **dpcode1**
  - Username: `dpcode1@gmail.com`
  - Password: `User@123`
- **dpcode2**
  - Username: `dpcode2@gmail.com`
  - Password: `User@123`
- **dpcode3**
  - Username: `dpcode3@gmail.com`
  - Password: `User@123`
- **dpcode4**
  - Username: `dpcode4@gmail.com`
  - Password: `User@123`

These users are seeded using Entity Framework Core's `HasData` method in `ExpenseDbContext`.
