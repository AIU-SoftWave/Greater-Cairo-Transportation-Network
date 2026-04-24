# Greedy Algorithms

## Purpose

Greedy algorithms make the best local decision at each step.
They are useful when the project needs quick, real-time decisions.

## Where they can be used
- traffic signal timing
- emergency vehicle priority
- local congestion reduction

## Signal timing

A greedy strategy may choose the intersection or direction with the highest immediate congestion relief.

## Emergency priority

A greedy strategy may give emergency vehicles immediate priority when critical facilities are involved.

## How the data fits
- `traffic_flow` tells where congestion is high
- `roads` shows what routes are available
- `locations.isCritical` shows critical nodes
- `road_maintenance` can also help in prioritization decisions

## Strengths and weaknesses

### Strengths
- fast
- simple
- good for real-time decisions

### Weaknesses
- may not give the global optimum
- may miss better long-term outcomes

## Service design

### Planned services
- `TrafficSignalService`
- `EmergencyPriorityService`

### What each service should do
- **TrafficSignalService**: pick the intersection or lane direction that reduces congestion the most right now
- **EmergencyPriorityService**: assign priority to emergency vehicles near critical locations

### What the services should return
- selected signal action or priority decision
- congestion score or urgency score
- short explanation for the choice

## Planned endpoints
- `GET /api/algorithms/traffic-signals?period=MORNING`
- `GET /api/algorithms/emergency-priority?from=1&to=F10`

## Beginner summary
Greedy means choosing the best-looking option right now, not necessarily the best one overall.

## Related pages
- [Overview](OVERVIEW.md)
- [Graph Data Behavior](GRAPH-DATA.md)
- [Diagrams](../DIAGRAMS/README.md)