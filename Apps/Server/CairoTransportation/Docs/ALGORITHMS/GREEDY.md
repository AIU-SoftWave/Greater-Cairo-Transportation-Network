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

## Implemented Services

### Traffic Signal Service

**Endpoint:** `GET /api/algorithms/traffic-signals?period=MORNING&topN=10&analyzeAllIntersections=false`

**Algorithm:** Greedy - prioritize highest congestion first

**What it does:**

- Analyzes traffic flow vs capacity for all roads in a period
- Aggregates repeated traffic rows per road before congestion scoring
- Sorts roads by congestion ratio (flow/capacity) descending
- Assigns longer green lights to most congested roads
- Generates signal timing recommendations

**Query parameters:**

- `period` - `MORNING`, `AFTERNOON`, `EVENING`, or `NIGHT`
- `topN` - number of roads to prioritize (1-50)
- `analyzeAllIntersections` - when `true`, ignores `topN` and evaluates all intersections with congestion data

**How it works:**

1. **Query traffic data:** Load `traffic_flow` for the period
2. **Calculate congestion:** `congestion_ratio = flow / capacity`
3. **Greedy sort:** Order by congestion descending
4. **Allocate green time:**
   - Base: 30 seconds
   - Additional: up to 90 seconds based on congestion
   - Critical (>100% capacity): maximum green time
5. **Return recommendations:** Priority-ranked signal timings

**Example Response:**

```json
{
  "algorithmName": "Traffic Signal Optimization (Greedy)",
  "success": true,
  "message": "Traffic signals optimized for MORNING: 8 roads prioritized by congestion.",
  "data": {
    "period": "MORNING",
    "roadsAnalyzed": 25,
    "intersectionsAnalyzed": 13,
    "intersectionsWithSignalRecommendations": 8,
    "signalRecommendations": 8,
    "totalCongestionScore": 8.4,
    "estimatedWaitTimeReductionPercent": 15.5,
    "signalTimings": [
      {
        "roadId": 42,
        "fromLocation": "Downtown",
        "toLocation": "Airport",
        "currentFlow": 850,
        "roadCapacity": 600,
        "congestionRatio": 1.42,
        "priorityRank": 1,
        "recommendedGreenDurationSeconds": 120,
        "recommendedCycleTimeSeconds": 120,
        "reason": "Critical congestion (142% of capacity) - maximum green time allocated"
      }
    ]
  }
}
```

### Emergency Priority Service (Planned)

**Concept:** Assign priority routing to emergency vehicles near critical locations.

**Endpoint:** `GET /api/algorithms/emergency-priority?from=1&to=F10`

**Planned functionality:**

- Find shortest path using A\*
- Prioritize critical locations (hospitals, fire stations)
- Suggest roads needing immediate clearance

## Planned endpoints

- `GET /api/algorithms/emergency-priority?from=1&to=F10`

## Beginner summary

Greedy means choosing the best-looking option right now, not necessarily the best one overall.

## Related pages

- [Overview](OVERVIEW.md)
- [Graph Data Behavior](GRAPH-DATA.md)
- [Diagrams](../DIAGRAMS/README.md)
