# Modules Documentation

This folder contains business-module documentation aligned with the current code structure under `Modules/`.

## Modules

- [API Conventions](API.md)
- [Routing](Routing/README.md)
- [Traffic Control](TrafficControl/README.md)
- [Network Management](NetworkManagement/README.md)
- [Transit Scheduling](TransitScheduling/README.md)
- [Maintenance Planning](MaintenancePlanning/README.md)

## Rules

- Business routes are primary and documented as the source of truth.
- Legacy routes are removed and should not be reintroduced.
- Module docs must be updated in the same PR when controller routes or public response shapes change.
