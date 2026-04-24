# Graph Data Behavior

## Graph view of the database

### Nodes
The `locations` table provides graph nodes.
Each location has:
- ID
- name
- type
- category
- population
- coordinates
- critical flag

### Edges
The `roads` table provides graph edges.
Each road has:
- source location
- destination location
- distance
- capacity
- condition
- existing vs potential flag
- construction cost

## Why direction matters
Roads are stored as directed edges.
That means `1 -> 3` is not automatically the same as `3 -> 1`.

## How traffic changes graph weight
The `traffic_flow` table gives a traffic volume per road and period.
That means an algorithm can calculate a dynamic cost for morning, evening, or other time slots.

## How maintenance fits into the graph
`road_maintenance` helps choose roads that should be repaired first.
This can be used with greedy or dynamic programming logic.

## How transit fits into the graph
`transport_routes` and `route_stops` can be treated as a separate layered network.
This is useful for public transport scheduling and transfer analysis.

## Beginner summary
Think of:
- locations as points on a map
- roads as arrows between points
- traffic as extra weight on those arrows

## Related pages
- [MST](MST.md)
- [Shortest Path](SHORTEST-PATH.md)
- [Dynamic Programming](DYNAMIC-PROGRAMMING.md)
- [Greedy Methods](GREEDY.md)
- [Diagrams](../DIAGRAMS/README.md)