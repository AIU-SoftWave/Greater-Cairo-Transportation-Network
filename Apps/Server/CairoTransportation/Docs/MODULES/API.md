# API Conventions

This page holds the shared API guidance that applies to every module.

## What the API layer does

The API layer:
- receives HTTP requests
- routes them to controller actions
- returns JSON responses
- exposes Swagger in development

## Swagger and OpenAPI

Swagger is the browser UI for exploring and testing the API.
OpenAPI is the machine-readable description behind that UI.

In this project:
- OpenAPI is enabled in `Program.cs`
- Swagger UI opens in development
- the browser launches to `/swagger`

## Response shape

The API returns entity-shaped JSON responses.
That means responses include:
- scalar fields
- foreign key IDs

And exclude:
- navigation objects
- recursive nested entities

This keeps the API output clean and avoids circular JSON problems.

### Example
A road response includes:
- `fromLocationId`
- `toLocationId`

But not:
- `fromLocation`
- `toLocation`

This is handled with `[JsonIgnore]` on navigation properties.

## Editing guidance

If you add new endpoints:
- keep controllers thin
- put logic in services
- document the endpoint in the module doc
- update Swagger if needed
