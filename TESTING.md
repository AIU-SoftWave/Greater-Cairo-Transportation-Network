# Testing & Demonstration Scenarios

This document outlines the manual test cases and sample scenarios designed to demonstrate the functionality and correctness of the transportation optimization algorithms. It serves as a guide for evaluating the system during the project presentation.

## Prerequisites for Testing
Ensure both the .NET backend API and the Next.js frontend are running.
1. Open the web interface in your browser.
2. The map of Greater Cairo with default road networks should be visible.

---

## Scenario 1: Standard vs. Emergency Routing
**Goal:** Prove that A* (Emergency Routing) selects different, potentially faster paths for critical scenarios compared to standard Dijkstra, and observe the performance differences.

**Steps:**
1. In the Map UI, set **Algorithm** to `Dijkstra`.
2. Click on a residential neighborhood (e.g., *Maadi*) as the Start point.
3. Click on a hospital (e.g., *Cairo University Hospital*) as the End point.
4. Note the **Distance**, **Est. Time**, and **Nodes Expanded** in the Results dashboard.
5. Change the **Algorithm** to `A*`.
6. Compare the Results.
   - **Expected Result:** A* should explore significantly fewer nodes (check the "Expanded" metric) due to its Euclidean distance heuristic, resulting in faster execution time while still finding an optimal path to the medical facility.

---

## Scenario 2: Time-Varying Traffic & Weather Effects
**Goal:** Demonstrate the Time-Varying Dijkstra algorithm's ability to avoid congestion and how extreme weather dynamically alters routing.

**Steps:**
1. Set **Algorithm** to `Time-Varying`.
2. Select **Period** as `Night`. Select a start and end point that crosses the city center.
3. Note the route taken and the **Est. Time**.
4. Change the **Period** to `Evening` (Rush Hour).
   - **Expected Result:** The algorithm should penalize congested central roads, potentially choosing a slightly longer but less traffic-heavy bypass route. The **Est. Time** will increase.
5. Open the **Simulation** panel and change **Weather Condition** to `Severe Storm`.
   - **Expected Result:** The route might change again, and the travel time will increase significantly (1.8x penalty) to reflect hazardous driving conditions.

---

## Scenario 3: Real-Time Accident Simulation
**Goal:** Verify that the system dynamically handles unexpected road closures (network partitioning) without crashing.

**Steps:**
1. Ensure a route is actively displayed on the map using `Dijkstra` or `Time-Varying`.
2. Change the **Algorithm** tab to `Simulation`.
3. On the map, **click a road segment** that is currently part of your active blue route.
   - **Expected Result:** The clicked road turns red/dashed (Closed). The routing algorithm will instantly recalculate, finding an alternate detour around the closed road.
4. Close multiple roads to form a blockade.
   - **Expected Result:** If the destination becomes completely unreachable, the UI will gracefully report "No path found" instead of throwing an error.

---

## Scenario 4: Urban Network Expansion (MST)
**Goal:** Demonstrate Prim's algorithm for cost-effective city expansion.

**Steps:**
1. In the control panel, click **Show MST**.
2. **Expected Result:** A green dashed network will overlay the map.
3. Observe the MST Results panel.
   - The algorithm connects all 35 nodes.
   - It prioritizes existing roads (Cost = 0) and high-population/critical facilities.
   - The total construction cost represents the minimum budget required to ensure every location in the dataset is reachable.

---

## Scenario 5: Maintenance Budget Allocation (0/1 Knapsack)
**Goal:** Prove the DP algorithm selects the optimal combination of roads to repair within a strict budget constraint.

**Steps:**
1. Select the **Maintenance** algorithm tab.
2. Set the **Budget** slider to `100` Million EGP. Click Calculate.
3. Note the selected roads and the Total Priority Score.
4. Increase the **Budget** to `150` Million EGP and Calculate.
   - **Expected Result:** The algorithm will select a completely different combination of roads. Rather than just adding one more road to the previous list, it evaluates the optimal subset, demonstrating true Knapsack behavior rather than a simple Greedy approach.

---

## Scenario 6: Transit Vehicle Scheduling (Bounded Knapsack DP)
**Goal:** Show optimal resource distribution across bus and metro lines.

**Steps:**
1. Select the **Transit** algorithm tab.
2. Set **Fleet Size** to `20` vehicles and Calculate.
   - **Expected Result:** The system assigns vehicles primarily to Metro lines due to their high passenger-to-vehicle efficiency.
3. Increase **Fleet Size** to `100`.
   - **Expected Result:** Once Metro lines reach their maximum capacity, the DP algorithm overflows the remaining vehicles into the Bus lines, maximizing total passenger coverage across the entire network.
4. Click **View Route Geometry** on any active transit route to see its physical path on the map.

---

## Scenario 7: Traffic Signal Optimization & Preemption (Greedy)
**Goal:** Demonstrate how the greedy algorithm minimizes wait times and handles emergency vehicle preemption.

**Steps:**
1. Select the **Signals** algorithm tab.
2. Choose the `Evening` period and Calculate.
   - **Expected Result:** The map shows intersection markers. Clicking a marker reveals the cycle timing. Congested roads receive proportionally longer green lights.
3. Switch back to **A*** and plot a route to a hospital.
4. Switch to **Signals** again and check intersections along that route.
   - **Expected Result:** The roads used by the emergency vehicle are flagged for "Preemption," receiving maximum priority in the signal cycle to ensure rapid transit.
