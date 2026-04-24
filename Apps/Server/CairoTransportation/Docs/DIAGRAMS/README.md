# Diagrams

This folder contains the visual diagrams for the project.

## What is inside
- `architecture.puml` – application architecture
- `data-model.puml` – entities and relationships
- `api-flow.puml` – how a request moves through the API
- `algorithm-map.puml` – how the algorithm modules fit together
- `graph-service-architecture.puml` – graph service as the foundation for algorithms (NEW)
- `ERD.png` – exported entity-relationship diagram

## Export rule
These PlantUML files are meant to be converted with `plantuml.bat` into PNG files.
The generated PNG output should be placed in:
- `png/`

## Why diagrams are useful
Diagrams make the project easier to understand for beginners and are helpful in the technical report.

## Recommended order
1. architecture
2. data model
3. ERD
4. API flow
5. algorithm map
6. graph service architecture (shows how algorithms depend on shared graph service)

## Links from other docs
- [Docs home](../README.md)
- [Start Here](../START-HERE/README.md)
- [Data Layer](../DATA/README.md)
- [API Layer](../API/README.md)
- [Algorithms](../ALGORITHMS/README.md)
- [Graph Service](../ALGORITHMS/GRAPH-SERVICE.md)
- [Project Goals](../PROJECT/README.md)