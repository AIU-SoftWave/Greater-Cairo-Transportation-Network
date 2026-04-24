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
- two-way flag
- construction cost

## Why direction matters
Roads are stored with a direction in the database.
That means `1 -> 3` is not automatically the same as `3 -> 1`.

## How two-way roads work
- `is_two_way = true` means the road can be used in both directions
- the graph service creates a reverse traversal edge in memory for that road
- algorithms like Dijkstra can then travel either direction without extra code
- `is_two_way = false` means only the original direction is available

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
- two-way roads as arrows that can be traversed in both directions

## Related pages
- [MST](MST.md)
- [Shortest Path](SHORTEST-PATH.md)
- [Dynamic Programming](DYNAMIC-PROGRAMMING.md)
- [Greedy Methods](GREEDY.md)
- [Diagrams](../DIAGRAMS/README.md)