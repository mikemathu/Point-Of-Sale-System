# Point of Sale System

**Accounts module**

The Accounts module encompasses various functionalities, including managing fiscal periods, ledger accounts, journal vouchers, and taxes within the system.

***Fiscal Periods:***
Fiscal periods enable structuring and manage financial data according to defined timeframes. Users can create, edit, and delete specific periods to align with their accounting needs.

***Ledger Accounts:***
Ledger accounts are the foundational elements of accounting, representing individual accounts for assets, liabilities, equity, revenue, and expenses. Users can perform CRUD (Create, Read, Update, Delete) operations on these accounts to maintain accurate financial records.

***Journal Voucher:***
This feature facilitates the creation and management of journal vouchers, essential for recording financial transactions. Users can add, modify, and delete vouchers, ensuring comprehensive transactional documentation.

***Taxes:***
The tax management functionality allows users to define, edit, and manage various tax configurations. It enables the setup and modification of tax-related information essential for accurate financial calculations and compliance.

**Security**

Each `SystemUser`, `Admin` and `Super Admin` is a `User`. To be a `User`, `User Registration` is required.

Each `User` is assigned none, one or more `User Role`.

Each `User Role` has set of `Privileges`. A `Privilege` defines whether `User` can invoke a particular action.

## Features
--> Features

## Technology Stack

- Frontend: HTML, CSS, JS
- Backend: C#
- Database: [Postgres](https://www.postgresql.org/download/)
- Data Access: [ADO.NET](https://learn.microsoft.com/en-us/dotnet/framework/data/adonet/ado-net-overview)
- Platform: [.NET 6.0](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) 
- API Tests: [Insomnia](https://insomnia.rest/download)


## How to Run

### Installation

    Clone and open the solution file in Visual Studio

### Create database

- Use pgAdmin to create an empty database named **Accounts**. 
- Import the **accounts.sql** file in the **wwwroot/AppData/Database** directory of this project.

### Configure connection string

Set a database connection string called `ConnectionStrings` in the **PointOfSaleSystem.Web** project's appsettings.json or use [Secrets](https://blogs.msdn.microsoft.com/mihansen/2017/09/10/managing-secrets-in-net-core-2-0-apps/)

Example config setting in appsettings.json for a database called `Accounts`:

```json
   "ConnectionStrings": {
    "DefaultConnection": "Host=localhost; Database=Accounts;  Username=postgres; Password=yourpassword"
  }
```
*"yourpassword"* - password to your database

### Configure startup in IDE

- Set the Startup Item in your IDE to **PointOfSaleSystem.Web** and start the server.

## Architecture
-> Images

## Application Overview
-->images

## Inspirations and Recommendations

### Database and SQL

- ["Database Lesson"](https://www.youtube.com/watch?v=4Z9KEBexzcM&list=PL1LIXLIF50uXWJ9alDSXClzNCMynac38g) Video Series, Dr. Daniel Soper

### ADO.NET

- ["ADO.NET Tutorial"](https://www.youtube.com/watch?v=aoFDyt8oG0k&list=PL6n9fhu94yhX5dzHunAI2t4kE0kOuv4D7) Video Series, kudvenkat

### C#

- ["Pro C# 9 with .NET 5, Tenth Edition"](https://www.amazon.com/Pro-NET-Foundational-Principles-Programming/dp/1484269381) Book,  Andrew Troelsen

### ASP.NET Core

- ["ASP.NET Core in Action, Second Edition"](https://www.manning.com/books/asp-net-core-in-action-second-edition) Book,  Andrew Lock

### Accounting
- ["Schaum's Outline of Principles of Accounting, 5th Edition"](https://www.youtube.com/watch?v=DYg2jT9aUG4) Book,  Joel Lerner
- ["Financial Accounting"](https://www.youtube.com/watch?v=DYg2jT9aUG4) Video, pmtycoon