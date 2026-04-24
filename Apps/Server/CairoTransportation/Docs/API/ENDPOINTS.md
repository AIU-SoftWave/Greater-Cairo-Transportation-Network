# Endpoints

## Locations
- `GET /api/locations`
- `GET /api/locations/{id}`

## Roads
- `GET /api/roads`
- `GET /api/roads/{id}`
- `GET /api/roads/from/{locationId}`
- `GET /api/roads/{roadId}/maintenance`

## Traffic
- `GET /api/traffic/road/{roadId}`
- `GET /api/traffic/period/{period}`

## Routes
- `GET /api/routes`
- `GET /api/routes/{id}`
- `GET /api/routes/{id}/stops`

## Response conventions
- `200 OK` when data is found
- `404 Not Found` when a single entity does not exist
- JSON output by default

## Notes
The endpoints are intentionally simple so they can later be reused by the algorithm modules.