# Point of Sale System

This project has 5 modules as outlined below.

**1. Accounts Module**

`Fiscal Period`, such as monthly, quarterly, or yearly intervals, serve as the structural framework for the accounting cycle. The accounting in this system operates within the context of the latest open and active period.

`Account` (such as Inventory, Revenue, etc.) is used to categorize and record financial transactions. Each `Account` is further divided into sub-accounts (like Inventory-Electronics, Revenue-Electronics, etc.). `Sub Account` provide further categorization within an `Account`, offering more detailed breakdowns of financial transactions.

Both `Account Type` (Assets, Liabilities, Equity, Revenue, and Expenses) and `Cashflow Category Type` (Operating Activities, Investing Activities, and Financing Activities) are foundational classifications used to categorize various accounts based on their nature and purpose within the system.

Both `Account Class` and `Cashflow Category` are used to further categorize accounts. `Account Class` classify accounts based on account types, offering specific classifications such as Current Assets, Current Liabilities, Fixed Assets, Sales Revenue, Cost of Goods Sold, etc. On the other hand, `Cashflow Category` categorize accounts according to cashflow category types, providing specific classifications such as Cash from Sales, Cash Payments to Suppliers, etc.

`Journal Voucher` serve as documentation for recording financial transactions, providing details of debit and credit journal voucher entries, transaction dates, descriptions, and references to other supporting documents such as receipt or invoices.

Within each `Journal Voucher`, there is/are journal voucher entry/(ies). `Journal Voucher Entry` document individual item by recording debit and credit linked to specific sub-accounts. Each `Journal Voucher Entry` includes details such as the debited and credited sub-accounts along with the transaction amounts.

The `Taxes` feature allows for configuring and managing various tax setups within the system. Configuration includes setting up tax rates, types, and their associated sub-accounts.(note that although taxes can be configured within the setup, they are currently not applied or included in the displayed item transactions)

**2. Inventory Module**

`Item` represent individual product available for sale within the system. Each `Item` is characterized by attributes such as name, cost, price, quantity, unit of measure, classification (`Item Class` and `Item Category`), and specific identifiers like code or barcode.

Each `Item` is linked to a specific `Unit of Measure`, establishing how the quantity of the `Item` is measured and sold.

`Item Class Type` (such as Electronics, Clothing, Home Appliances, etc.) are foundational classifications used to categorize various items according to their characteristics. These item class types are further categorized by item classes, providing more specific classifications.

Both `Item Class` and `Item Category` provide structured classifications for grouping and organizing items based on their characteristics or types.

`Item` are associated with relevant sub-accounts, enabling the financial tracking of inventory, cost of sale, and revenue within the system. 

System Functionality:
When the quantity of an `Item` is updated, the system automatically generates and posts a corresponding `Journal Voucher` to reflect this change. The balance amount linked to the items' inventory `Sub Account` is adjusted accordingly, either increased or decreased based on the updated quantity of the `Item`.

**3. Point of Sale Module**

An `Item` is sold, capturing details such as item name, quantities, prices, and payment methods.

System Functionality:
Each sales transaction involves one or more inventory items, reducing the available stock by the sold quantities. Additionally, an automatic `Journal Voucher` is posted, adjusting the balance amounts linked to the inventory, Cost of Sale, and Revenue sub-accounts for each `Item`.

**4. Security Module**

Super Admin, Admin and User are consideres as `System User`. To become a `System User`, registration is required.

Each `System User` can be assigned none, one, or multiple User `Roles`.

Each user `Role` encompasses a set of privileges. `Privilege` define whether a System User can perform specific actions.


**5. Reports Module**

Generates reports based on both recorded financial information along with sales data.(note: Not yet implemented)


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

- Use pgAdmin(or other) to create an empty database named **Accounts**. 
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





## Application Overview
Get Request
<img src="https://github.com/mikemathu/Point-Of-Sale-System/blob/main/PointOfSaleSystem.Web/wwwroot/AppData/images/API/GetRequest.png">

Post Request
<img src="https://github.com/mikemathu/Point-Of-Sale-System/blob/main/PointOfSaleSystem.Web/wwwroot/AppData/images/API/PostRequest.png">

Point of Sale
<img src="https://github.com/mikemathu/Point-Of-Sale-System/blob/main/PointOfSaleSystem.Web/wwwroot/AppData/images/POS/PointofSale.png">

Point of Sale
<img src="https://github.com/mikemathu/Point-Of-Sale-System/blob/main/PointOfSaleSystem.Web/wwwroot/AppData/images/POS/POS.png">

Fiscal Period
<img src="https://github.com/mikemathu/Point-Of-Sale-System/blob/main/PointOfSaleSystem.Web/wwwroot/AppData/images/Accounts/Fiscalperiods.png">

Journal Voucher
<img src="https://github.com/mikemathu/Point-Of-Sale-System/blob/main/PointOfSaleSystem.Web/wwwroot/AppData/images/Accounts/JournalVouchers.png">

Journal Voucher Entries
<img src="https://github.com/mikemathu/Point-Of-Sale-System/blob/main/PointOfSaleSystem.Web/wwwroot/AppData/images/Accounts/JournalVouchersEntries.png">

Ledger Voucher
<img src="https://github.com/mikemathu/Point-Of-Sale-System/blob/main/PointOfSaleSystem.Web/wwwroot/AppData/images/Accounts/LedgerAccounts.png">

Taxes
<img src="https://github.com/mikemathu/Point-Of-Sale-System/blob/main/PointOfSaleSystem.Web/wwwroot/AppData/images/Accounts/Taxes.png">

Inventory
<img src="https://github.com/mikemathu/Point-Of-Sale-System/blob/main/PointOfSaleSystem.Web/wwwroot/AppData/images/Inventory/Taxes.png">

Units Of Measure
<img src="https://github.com/mikemathu/Point-Of-Sale-System/blob/main/PointOfSaleSystem.Web/wwwroot/AppData/images/Inventory/UnitsofMeasures.png">

Privileges
<img src="https://github.com/mikemathu/Point-Of-Sale-System/blob/main/PointOfSaleSystem.Web/wwwroot/AppData/images/Security/Privileges.png">

Roles
<img src="https://github.com/mikemathu/Point-Of-Sale-System/blob/main/PointOfSaleSystem.Web/wwwroot/AppData/images/Security/Roles.png">

Users
<img src="https://github.com/mikemathu/Point-Of-Sale-System/blob/main/PointOfSaleSystem.Web/wwwroot/AppData/images/Security/Users.png">

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
