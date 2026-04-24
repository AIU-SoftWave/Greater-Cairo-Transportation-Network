# Seeding

## What seeding means

Seeding is the process of inserting initial data into the database automatically.

## Project behavior

This project reads the file:
- `Data/TablesData.sql`

At startup, the app checks whether the database is empty.
If it is empty, the SQL file is executed once.

## Why this is useful

It gives you:
- a repeatable dataset
- no manual data entry
- a working demo environment right after startup

## What the seed file contains
- locations
- roads
- traffic flows
- transport routes
- route stops
- transport demand
- road maintenance

## Important rule
The seed data is only meant to run once on an empty database.
That prevents duplicate rows when the app restarts.

## Editing guidance
If you change the seed data:
- update `TablesData.sql`
- delete the SQLite database file if needed
- run the app again

## Beginner explanation
The seeder is just startup code that says:
- if there is no data, insert the starting dataset