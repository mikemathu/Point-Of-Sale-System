# Point of Sale System

This project has 5 modules as outlined below.

**1. Security Module**

Each `SystemUser`, `Admin` and `Super Admin` is a `User`. To be a `User`, `User Registration` is required.

Each `User` is assigned none, one or more `User Role`.

Each `User Role` has set of `Privileges`. A `Privilege` defines whether `User` can invoke a particular action.

**2. Inventory module**

The Inventory module manages the cataloging, tracking, and control of products, facilitating efficient management of stock and sales within the system.
Key Entities:

    Items:
        Each Item represents a product available in the inventory, holding details such as name, cost, price, quantity, and associated attributes like unit of measure, item class, and item category.

    Unit of Measure:
        Defines the measurement units associated with items, enabling uniformity in quantifying different types of products.

    Item Class and Item Category:
        Item Class and Category provide a structured categorization for items, aiding in organizing and classifying products based on their characteristics and types.

    Sub-Accounts:
        Sub-Accounts within the inventory context help in associating items, item classes, or categories with specific accounts (e.g., Asset, Cost of Sale, Revenue).

Relationships:

    Item and Unit of Measure:
        Each Item is associated with a specific Unit of Measure, defining how the quantity of the item is measured or sold.

    Item and Item Class/Category:
        Items are categorized under specific Item Classes and Categories, providing a systematic grouping for easier management and analysis.

    Items and Accounts:
        Items are linked to relevant Sub-Accounts, allowing for proper financial tracking and association with accounting aspects such as asset management, cost of sale, and revenue generation.

System Functionality:

    Inventory Management:
        Allows the addition, modification, and removal of items.
        Tracks and manages quantities, costs, prices, and other attributes associated with inventory items.

    Categorization and Structuring:
        Enables the organization and grouping of items through Item Classes and Categories for better management and reporting.

    Financial Integration:
        Associates items with financial aspects by linking them to Sub-Accounts, ensuring accurate accounting and financial tracking.

    Reporting and Analysis:
        Provides functionalities for generating reports and conducting analyses based on inventory data, facilitating informed decision-making.

    Permissions and Access Control (if applicable):
        Might implement access controls defining user roles and privileges related to inventory operations, ensuring proper control and security within the module.

**3. Accounts module**

The Accounts module encompasses various functionalities, including managing fiscal periods, ledger accounts, journal vouchers, and taxes within the system.

***Fiscal Periods:***
Fiscal periods enable structuring and manage financial data according to defined timeframes. Users can create, edit, and delete specific periods to align with their accounting needs.

***Ledger Accounts:***
Ledger accounts are the foundational elements of accounting, representing individual accounts for assets, liabilities, equity, revenue, and expenses. Users can perform CRUD (Create, Read, Update, Delete) operations on these accounts to maintain accurate financial records.

***Journal Voucher:***
This feature facilitates the creation and management of journal vouchers, essential for recording financial transactions. Users can add, modify, and delete vouchers, ensuring comprehensive transactional documentation.

***Taxes:***
The tax management functionality allows users to define, edit, and manage various tax configurations. It enables the setup and modification of tax-related information essential for accurate financial calculations and compliance.

**4. Point of Sale module**



## Features
--> Features

## Technology Stack

- Frontend: ASP.NET Core MVC with HTML, CSS, JS
- Backend: ASP.NET Core Web API with C#
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