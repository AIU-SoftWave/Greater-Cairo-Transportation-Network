# ASP.NET Core Basics

ASP.NET Core is the web framework used to build the API.
This page explains how it works in this project.

## What ASP.NET Core does
- hosts the web app
- receives HTTP requests
- routes requests to controllers
- returns HTTP responses
- configures dependency injection

## How this project uses ASP.NET Core

### `Program.cs`
This is the app entry point.
It wires up:
- controllers
- EF Core DbContext
- services
- migrations
- seeding
- Swagger/OpenAPI

### Dependency injection
ASP.NET Core creates and injects services for you.
In this project, controllers receive service classes through constructor injection.

### Swagger/OpenAPI
Swagger documents the API and lets you test endpoints in the browser.

### Environment support
The app can behave differently in development and production.
Here it is mainly used to:
- enable Swagger in development
- open the browser to `/swagger`

## Important framework concepts

### Middleware
Middleware is the pipeline that processes requests before they reach controllers.

### Routing
Routing matches URLs like `/api/road-network/{id}` to the correct controller action.

### Action results
Controller methods return `IActionResult` so they can send:
- `200 OK`
- `404 Not Found`
- other HTTP responses

## Beginner mental model
Think of ASP.NET Core as the outer shell of the app:
- it starts the server
- it receives the request
- it passes work to the service layer
- it returns the response to the client
