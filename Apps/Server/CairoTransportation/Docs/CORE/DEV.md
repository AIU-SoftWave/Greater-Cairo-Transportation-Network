# Development Guide

This page explains how to work on the project as a developer.

## What is here
- setup instructions
- libraries used
- contribution notes

## Recommended workflow
1. Edit a model or service.
2. Create or update a migration if the schema changes.
3. Run the app.
4. Check Swagger.
5. Verify the response shape.

## Setup

### Requirements
- .NET 10 SDK
- Visual Studio 2026 or compatible IDE
- SQLite support through EF Core packages

### Run the app
1. Open the solution.
2. Make sure `appsettings.json` contains the SQLite connection string.
3. Run the application.

On startup the app will:
- apply migrations
- seed the database if it is empty
- start the API
- expose Swagger in development

### Useful URLs
- `http://localhost:5208/swagger`
- `https://localhost:7167/swagger`

## Libraries used

### ASP.NET Core
Used for:
- hosting the web app
- routing HTTP requests
- controller actions
- dependency injection

### Entity Framework Core
Used for:
- mapping C# models to database tables
- querying the database
- applying migrations
- seeding the database

### SQLite provider
Used for:
- local file-based database storage
- simple development setup

### Swagger / OpenAPI
Used for:
- documentation
- testing endpoints in the browser

### System.Text.Json
Used for:
- JSON serialization
- returning API responses

## Contribution notes

### General rules
- keep files small and readable
- use the existing folder structure
- add documentation when behavior changes
- keep responses clean and simple

### When you change a model
- update the model class
- update the DbContext if needed
- create a migration
- run the app
- check the Swagger response

### When you add an algorithm
- place it in a dedicated folder
- keep the service focused on one responsibility
- add a controller endpoint if the result should be testable
- document the algorithm in `Docs/MODULES/`
