# Traffic Control Module

## Purpose

Handles traffic flow lookup, traffic policy multipliers, and signal optimization.

## Controllers and Base Routes

- `TrafficController` -> `GET /api/traffic-monitoring/*`
- `TrafficPeriodMultipliersController` -> `GET /api/traffic-policy/*`
- `TrafficSignalController` -> `GET /api/signal-optimization`

## Endpoints

- `GET /api/traffic-monitoring/road/{roadId}`
- `GET /api/traffic-monitoring/period/{period}`
- `GET /api/traffic-policy`
- `GET /api/traffic-policy/{period}`
- `GET /api/signal-optimization?period=MORNING&topN=10&analyzeAllIntersections=false`

## Services

- `TrafficService`
- `TrafficSignalService`
