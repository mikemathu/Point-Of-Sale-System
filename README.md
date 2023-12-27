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

**System Functionality:**
When the quantity of an `Item` is updated, the system automatically generates and posts a corresponding `Journal Voucher` to reflect this change. The balance amount linked to the items' inventory `Sub Account` is adjusted accordingly, either increased or decreased based on the updated quantity of the `Item`.

**3. Point of Sale Module**

An `Item` is sold, capturing details such as item name, quantities, prices, and payment methods.

**System Functionality:** 
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
- Database: [PostgreSQL](https://www.postgresql.org/download/)
- Data Access: [ADO.NET](https://learn.microsoft.com/en-us/dotnet/framework/data/adonet/ado-net-overview)
- Platform: [.NET 6.0](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) 
- API Tests: [Insomnia](https://insomnia.rest/download)


## How to Run

### Installation

    clone and open the solution file in Visual Studio

### Create database

- Use pgAdmin(or other) to create an empty database named **Accounts**. 
- Import the **Accounts.sql** file from the **wwwroot/AppData/Database** directory of this project.

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
- Log in with user name **Super Admin** and password **123**

### Configure an API client for interacting with the API

- Open Insomnia (or another tool) and import the **PointOfSale.json** file from the 'wwwroot/AppData' directory of this project.
- Ensure that you make the POST request along with the JSON, as shown below, to authenticate.

<img src="https://github.com/mikemathu/Point-Of-Sale-System/blob/main/PointOfSaleSystem.Web/wwwroot/AppData/images/Authenticate.png">

## Architecture 
(not maintained, only for demonstration)

<img src="https://github.com/mikemathu/Point-Of-Sale-System/blob/main/PointOfSaleSystem.Web/wwwroot/AppData/images/Architecture.png">

## ER Diagrams

Accounts Module
<img src="https://github.com/mikemathu/Point-Of-Sale-System/blob/main/PointOfSaleSystem.Web/wwwroot/AppData/images/ERDiagrams/Accounts.png">

Inventory Module
<img src="https://github.com/mikemathu/Point-Of-Sale-System/blob/main/PointOfSaleSystem.Web/wwwroot/AppData/images/ERDiagrams/Inventory.png">

Security Module
<img src="https://github.com/mikemathu/Point-Of-Sale-System/blob/main/PointOfSaleSystem.Web/wwwroot/AppData/images/ERDiagrams/Security.png">

Entire Project ER Diagram
<img src="https://github.com/mikemathu/Point-Of-Sale-System/blob/main/PointOfSaleSystem.Web/wwwroot/AppData/images/ERDiagrams/EntireERDigram.png">

## API Requests

Get Request
<img src="https://github.com/mikemathu/Point-Of-Sale-System/blob/main/PointOfSaleSystem.Web/wwwroot/AppData/images/API/GetRequest.png">

Post Request
<img src="https://github.com/mikemathu/Point-Of-Sale-System/blob/main/PointOfSaleSystem.Web/wwwroot/AppData/images/API/PostRequest.png">

## Application Overview

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

Ledger Accounts
<img src="https://github.com/mikemathu/Point-Of-Sale-System/blob/main/PointOfSaleSystem.Web/wwwroot/AppData/images/Accounts/LedgerAccounts.png">

Taxes
<img src="https://github.com/mikemathu/Point-Of-Sale-System/blob/main/PointOfSaleSystem.Web/wwwroot/AppData/images/Accounts/Taxes.png">

Inventory
<img src="https://github.com/mikemathu/Point-Of-Sale-System/blob/main/PointOfSaleSystem.Web/wwwroot/AppData/images/Inventory/Inventory.png">

Units Of Measure
<img src="https://github.com/mikemathu/Point-Of-Sale-System/blob/main/PointOfSaleSystem.Web/wwwroot/AppData/images/Inventory/UnitsofMeasures.png">

Roles
<img src="https://github.com/mikemathu/Point-Of-Sale-System/blob/main/PointOfSaleSystem.Web/wwwroot/AppData/images/Security/Roles.png">

Privileges
<img src="https://github.com/mikemathu/Point-Of-Sale-System/blob/main/PointOfSaleSystem.Web/wwwroot/AppData/images/Security/Privileges.png">

Users
<img src="https://github.com/mikemathu/Point-Of-Sale-System/blob/main/PointOfSaleSystem.Web/wwwroot/AppData/images/Security/Users.png">

## Inspirations and Recommendations

### Database and SQL

- ["Database Lesson"](https://www.youtube.com/watch?v=4Z9KEBexzcM&list=PL1LIXLIF50uXWJ9alDSXClzNCMynac38g) Video Series, Dr. Daniel Soper

### ADO.NET

- ["ADO.NET Tutorial"](https://www.youtube.com/watch?v=aoFDyt8oG0k&list=PL6n9fhu94yhX5dzHunAI2t4kE0kOuv4D7) Video Series, kudvenkat

### C#

- ["Pro C# 10 with .NET 6, Eleventh Edition"](https://www.amazon.com/Pro-NET-Foundational-Principles-Programming-dp-1484278682/dp/1484278682/ref=dp_ob_title_bk) Book,  Andrew Troelsen

### ASP.NET Core

- ["ASP.NET Core in Action, Third Edition"](https://www.manning.com/books/asp-net-core-in-action-third-edition) Book,  Andrew Lock

### Accounting
- ["Schaum's Outline of Principles of Accounting, 5th Edition"](https://www.mheducation.com/highered/product/schaum-s-outline-principles-accounting-i-fifth-edition-lerner-cashin/9780071635387.html) Book,  Joel Lerner
- ["Financial Accounting"](https://www.youtube.com/watch?v=DYg2jT9aUG4) Video, pmtycoon
