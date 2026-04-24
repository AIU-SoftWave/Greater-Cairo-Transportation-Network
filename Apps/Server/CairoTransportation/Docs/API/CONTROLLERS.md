# Controllers

Controllers are the HTTP entry points of the app.

## Current controllers
- `LocationsController`
- `RoadsController`
- `TrafficController`
- `RoutesController`

## What controllers should do

Controllers should:
- accept request parameters
- call services
- return HTTP responses

Controllers should not:
- contain database logic
- contain algorithm logic
- build SQL by hand

## Beginner summary
A controller is like the front desk of the API.
It gets the request and sends the work to the right service.