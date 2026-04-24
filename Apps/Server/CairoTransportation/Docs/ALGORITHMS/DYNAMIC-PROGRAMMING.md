# Dynamic Programming

## Overview

Dynamic Programming (DP) is used for optimization problems that can be broken into smaller subproblems. The key insight: solve each subproblem once, store the result, and reuse it. This avoids exponential recomputation.

## Implemented Algorithm

### Maintenance Planning Service (0/1 Knapsack)

**Endpoint:** `GET /api/algorithms/maintenance-plan?budget=10000000`

**Algorithm:** 0/1 Knapsack Dynamic Programming

**Problem:** Given a maintenance budget, select which roads to repair to maximize total priority score while staying within budget.

**Data Used:**

- `road_maintenance` table: `priority`, `estimated_cost`
- `roads` table: `condition`, `from_location_id`, `to_location_id`
- `locations` table: `name` (for identifying roads)

**How It Works:**

1. **Compute value scores** for each road:

   ```
   value = priority × (1 + urgency)
   urgency = (100 - condition) / 100  // Lower condition = higher urgency
   ```

2. **0/1 Knapsack DP**:

   ```
   dp[b] = maximum value achievable with budget b

   For each road i:
     For b from max_budget down to cost[i]:
       dp[b] = max(dp[b], dp[b - cost[i]] + value[i])
   ```

3. **Backtrack** to find which roads were selected

4. **Return** selected roads, total cost, total priority score, expected condition improvement

**Why 0/1 Knapsack?**

- Each road can either be repaired (1) or not (0) — binary choice
- Budget constraint is the "knapsack capacity"
- Priority score is the "item value"
- Repair cost is the "item weight"

**Complexity:**

- Time: O(n × budget) where n = number of candidate roads
- Space: O(budget) using 1D array optimization

**Example Response:**

```json
{
  "algorithmName": "Maintenance Planning (0/1 Knapsack DP)",
  "success": true,
  "message": "Maintenance plan generated: 12 roads selected with total priority score 156 within budget $10,000,000.",
  "data": {
    "budget": 10000000,
    "totalCost": 9850000,
    "remainingBudget": 150000,
    "totalPriorityScore": 156,
    "selectedRoadCount": 12,
    "totalCandidateRoads": 25,
    "expectedConditionImprovement": 347.5,
    "selectedRoads": [
      {
        "roadId": 15,
        "fromLocation": "Downtown",
        "toLocation": "Maadi",
        "currentCondition": 45,
        "estimatedCost": 1200000,
        "priority": 10,
        "expectedNewCondition": 72.5,
        "reason": "Selected by 0/1 Knapsack optimization for max priority within budget"
      }
    ],
    "notSelectedRoads": [...]
  }
}
```

**Files:**
| File | Purpose |
|------|---------|
| `Services/Algorithms/MaintenancePlanning/MaintenancePlanningService.cs` | Core 0/1 Knapsack implementation |
| `Services/Algorithms/MaintenancePlanning/Contracts/IMaintenancePlanningService.cs` | Service interface |
| `Services/Algorithms/MaintenancePlanning/DTOs/MaintenancePlanningResultDto.cs` | Response DTOs |
| `Controllers/MaintenancePlanningController.cs` | API endpoint |

---

## Planned Algorithms

### Transit Scheduling (Not Yet Implemented)

**Concept:** Optimize bus/metro frequency across routes given vehicle constraints and passenger demand.

**Algorithm:** DP Resource Allocation or Network Flow

**Data:**

- `transport_routes` — route structure
- `route_stops` — stop sequences
- `transport_demand` — passenger demand between locations

**Output:**

- Vehicle assignments per route
- Frequency (trips per hour) per route
- Total capacity vs. demand analysis

---

## When to Use Dynamic Programming

| Use DP When                             | Example                                                        |
| --------------------------------------- | -------------------------------------------------------------- |
| Problem has **optimal substructure**    | Best plan with budget B uses best plan with budget B - cost[i] |
| Problem has **overlapping subproblems** | Same budget level checked multiple times                       |
| Need **exact optimal solution**         | 0/1 knapsack guarantees maximum value                          |
| Constraints are **reasonable size**     | Budget can be discretized (cents)                              |

## Alternative: Greedy Approach

For some problems, a greedy approach (pick highest priority/cost ratio first) is faster but may not yield the optimal solution. The 0/1 knapsack specifically requires DP because greedy fails on certain cases.

**Example where greedy fails:**

- Budget: 10
- Road A: cost 6, priority 8 (ratio 1.33)
- Road B: cost 5, priority 5 (ratio 1.0)
- Road C: cost 5, priority 5 (ratio 1.0)

Greedy picks A → total priority 8 (can't afford B or C)
Optimal picks B + C → total priority 10

## Related Pages

- [Overview](OVERVIEW.md) — Algorithm system architecture
- [Greedy Methods](GREEDY.md) — Faster but potentially suboptimal alternatives
- [MST](MST.md) — Another optimization algorithm for network design
