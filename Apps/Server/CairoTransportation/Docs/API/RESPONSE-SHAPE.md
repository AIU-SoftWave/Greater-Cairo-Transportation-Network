# Response Shape

## Current response style
The API returns entity-shaped JSON responses.
That means responses include:
- scalar fields
- foreign key IDs

And exclude:
- navigation objects
- recursive nested entities

## Why this is useful
This keeps the API output clean and easy to read.
It also avoids circular JSON problems.

## Example
A road response includes:
- `fromLocationId`
- `toLocationId`

But not:
- `fromLocation`
- `toLocation`

## How this is done
Navigation properties in the models use `[JsonIgnore]`.

## Beginner explanation
This is a simple way to return just the important data without dumping the whole object graph.