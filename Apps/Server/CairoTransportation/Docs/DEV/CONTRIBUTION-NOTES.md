# Contribution Notes

## General rules
- keep files small and readable
- use the existing folder structure
- add documentation when behavior changes
- keep responses clean and simple

## When you change a model
- update the model class
- update the DbContext if needed
- create a migration
- run the app
- check the Swagger response

## When you add an algorithm
- place it in a dedicated folder
- keep the service focused on one responsibility
- add a controller endpoint if the result should be testable
- document the algorithm in `Docs/ALGORITHMS/`

## Beginner advice
Use the docs folders as a map.
Add new markdown files instead of making one huge document.