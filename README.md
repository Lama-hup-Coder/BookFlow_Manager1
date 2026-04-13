# 📚 BookFlow Manager - Library Management System

A sophisticated Library Management System built with **C#** and **.NET**, implementing a clean **3-Tier Architecture**. This project is designed to handle library operations efficiently while maintaining a highly organized and scalable codebase.

## 🏗️ Architecture Overview
The project is divided into four main layers to ensure separation of concerns:
* **Library_DataAccess:** Handles all direct interactions with the SQL Server database.
* **Library_Business:** Contains the logic, validation rules, and bridge between data and UI.
* **Library_Common:** A shared layer for data models and global utilities.
* **Library_TestConsole:** A dedicated environment for testing logic and backend functionality.

## 🚀 Features (Work in Progress)
* **Book Management:** Full CRUD operations for library assets.
* **Advanced Logic:** Implementing **Delegates (Action, Func, Predicate)** for calculations and search filters.
* **Event-Driven Design:** Utilizing **Events** and **EventArgs** for system notifications.
* **Database Integration:** SQL-backed storage with optimized stored procedures.

## 🛠️ Technologies Used
* **Language:** C#
* **Framework:** .NET / Windows Forms
* **Database:** SQL Server
* **Design Pattern:** 3-Tier Architecture

## 📂 Database Setup
The database schema can be found in the `Database/` folder. Execute the `.sql` script in your SQL Server Management Studio to set up the environment.

## 📈 Learning Journey
This project serves as a practical application for advanced C# concepts, including Events, Delegates, and asynchronous programming.
