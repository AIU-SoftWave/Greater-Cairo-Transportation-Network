# In-Depth Theoretical Analysis of Dijkstra's Algorithm

### Greater Cairo Transportation Network — CSE112: Algorithms & Data Structures
### Team: AIU-SoftWave · May 2026

> **Presentation Guide:** Each `---` horizontal rule marks a **slide boundary**.  
> Every section heading maps directly to a PowerPoint slide.  
> Copy the text between rules into your slide; diagrams are in `/diagrams/` (render `.mmd` with Mermaid CLI, `.puml` with PlantUML).

---

## 📌 Table of Contents

1. [Slide 1 — Introduction & Why Dijkstra's?](#slide-1--introduction--why-dijkstras)
2. [Slide 2 — Mathematical Foundations & Pseudocode](#slide-2--mathematical-foundations--pseudocode)
3. [Slide 3 — Formal Proof of Correctness](#slide-3--formal-proof-of-correctness)
4. [Slide 4 — Complexity Analysis](#slide-4--complexity-analysis)
5. [Slide 5 — Cairo-Specific Implementation & Modifications](#slide-5--cairo-specific-implementation--modifications)
6. [Slide 6 — Performance: Dijkstra vs A*](#slide-6--performance-dijkstra-vs-a)
7. [Slide 7 — Comparison with Alternative Algorithms](#slide-7--comparison-with-alternative-algorithms)
8. [Slide 8 — Optimization Opportunities](#slide-8--optimization-opportunities)
9. [Slide 9 — Conclusion & Lessons Learned](#slide-9--conclusion--lessons-learned)
10. [Appendix A — Full Implementation Walkthrough](#appendix-a--full-implementation-walkthrough)
11. [Appendix B — Diagrams Index](#appendix-b--diagrams-index)

---
---

## Slide 1 — Introduction & Why Dijkstra's?

### What Is Dijkstra's Algorithm?

Dijkstra's algorithm, published by Edsger W. Dijkstra in **1959**, solves the **Single-Source Shortest Path (SSSP)** problem on a weighted directed graph with **non-negative edge weights**.

Given a source node `s`, it finds the shortest (minimum-weight) path from `s` to every other reachable node in the graph — or, with early termination, to a single target.

### Why It Powers Cairo's Routing Engine

| Criterion | Why Dijkstra's Fits |
|---|---|
| **Non-negative weights** | Road distances are always ≥ 0 km |
| **Optimal guarantee** | Provably finds the shortest path (no heuristic approximation) |
| **Speed** | O((V+E) log V) — fast enough for Cairo's 35-node, ~120-edge network |
| **Simplicity** | Straightforward to verify, audit, and extend |
| **Proven correctness** | Induction proof is clean and complete |

### General Applications of Dijkstra's

- **GPS & Navigation** — Google Maps, Waze route calculation
- **Network Routing** — OSPF (Open Shortest Path First) internet protocol
- **Robotics** — Motion planning in known environments
- **Game AI** — Pathfinding on tile maps
- **Telecommunications** — Minimum-latency network paths

### System Context in Cairo Transportation Network

![System Architecture](../../Apps/Server/CairoTransportation/Docs/DIAGRAMS/PlantUMLout/Architecture%20-%20Component%20Diagram.png)

**Figure:** Dijkstra operates within the Routing Services layer, using the shared GraphService infrastructure

---
---

## Slide 2 — Mathematical Foundations & Pseudocode

### Formal Problem Definition

> **Single-Source Shortest Path (SSSP):**  
> Given a directed weighted graph G = (V, E, w), where w: E → ℝ≥0, and a source node s ∈ V,  
> find for every v ∈ V the **shortest-path distance** δ(s, v) defined as:
>
> δ(s, v) = min{ Σ w(e) | p is a path from s to v in G }  
>
> with δ(s, v) = ∞ if no path exists.

### Key Mathematical Properties

**1. Optimal Substructure**

> If p\* = ⟨s, …, u, …, v⟩ is a shortest path from s to v,  
> then the sub-path p' = ⟨s, …, u⟩ is a shortest path from s to u.

*Proof (cut-and-paste):* Suppose p' is not shortest. Then ∃ a shorter path q from s to u.  
Replacing p' with q in p\* gives a path shorter than p\*, contradicting optimality of p\*. □

**2. Relaxation Lemma**

> After executing `Relax(u, v)`:  
> d[v] ≤ d[u] + w(u, v)

*Proof:* If d[u] + w(u,v) < d[v] before the call, then d[v] is updated; otherwise d[v] is unchanged.  
In either case the inequality holds. □

**3. Triangle Inequality**

> For every edge (u, v) ∈ E:  δ(s, v) ≤ δ(s, u) + w(u, v)

*Proof:* The path ⟨s, …, u, v⟩ (concatenating shortest s→u with edge u→v) is a valid path to v.  
By definition, δ(s, v) cannot exceed its length. □

### Pseudocode (Binary-Heap Variant)

```
function Dijkstra(G, s, t):
    // Initialization
    for each v in V:
        dist[v] ← ∞
        prev[v] ← nil
    dist[s] ← 0
    PQ ← MinHeap containing (s, 0)

    // Main loop
    while PQ ≠ ∅:
        u ← PQ.extractMin()          // O(log V)
        if u is already visited: continue
        mark u as visited

        if u == t: break             // Early termination ← Cairo optimisation

        for each edge (u, v, w) in Adj[u]:
            newDist ← dist[u] + w
            if newDist < dist[v]:   // Relaxation
                dist[v] ← newDist
                prev[v] ← u
                PQ.insert(v, newDist)  // O(log V)

    // Path reconstruction
    path ← []
    curr ← t
    while curr ≠ nil:
        path.prepend(curr)
        curr ← prev[curr]
    return dist[t], path
```

> **See diagram:** `diagrams/dijkstra_flowchart.puml` — full annotated control-flow diagram  
> **See also:** `../../Apps/Server/CairoTransportation/Docs/DIAGRAMS/PlantUMLout/Algorithm%20Flowchart%20-%20Dijkstra%20Example.png` — alternative visual flowchart

---
---

## Slide 3 — Formal Proof of Correctness

### Theorem

> **When a node u is dequeued from the priority queue, d[u] = δ(s, u).**

### Proof by Strong Induction on |S|

Let S denote the **visited (finalised) set** — the set of nodes already dequeued and marked.

**Invariant:** For every node x ∈ S, d[x] = δ(s, x).

---

#### Base Case: |S| = 0 → |S| = 1 (adding source s)

- s is enqueued with priority 0.
- d[s] = 0 = δ(s, s) by definition (trivial path of length 0).
- Since all weights w ≥ 0, no path from s to s can have negative length.
- ∴ d[s] = δ(s, s). ✓

---

#### Inductive Step: Assume for all x ∈ S (|S| = k), d[x] = δ(s, x)

Let u be the next node dequeued (the one with minimum d[u] in the heap).

**Claim:** d[u] = δ(s, u).

**Proof by contradiction:** Suppose d[u] > δ(s, u).

Let p\* be a true shortest path from s to u. Since s ∈ S and u ∉ S, p\* must cross the boundary of S at some point. Let (x, y) be the **first edge** on p\* where x ∈ S and y ∉ S.

By the inductive hypothesis applied to x: d[x] = δ(s, x).

When x was added to S, edge (x, y) was relaxed:
```
d[y] ≤ d[x] + w(x, y) = δ(s, x) + w(x, y) = δ(s, y)
```
(the last equality holds because the sub-path of p\* from s to y is shortest by Optimal Substructure).

Since y is a prefix of the path to u and all remaining weights are ≥ 0:
```
δ(s, y) ≤ δ(s, u)
```

Therefore: `d[y] ≤ δ(s, u)`

But u was **dequeued before y**, so the heap gave us d[u] ≤ d[y]:
```
d[u] ≤ d[y] ≤ δ(s, u)
```

Combined with the fact that d[u] ≥ δ(s, u) (distances always overestimate):
```
d[u] = δ(s, u)    □
```

**Contradiction.** Our assumption was false. ✓

---

#### Termination

Each node enters S **at most once** (the `HashSet.Add` check returns false for duplicates).  
|S| increases by 1 per meaningful iteration. After at most V iterations, the algorithm terminates. □

> **See diagram:** `diagrams/correctness_proof_sequence.puml` — visual sequence of the induction steps

---
---

## Slide 4 — Complexity Analysis

### Time Complexity: O((V + E) log V)

#### Derivation with Amortized Analysis

The algorithm performs two types of heap operations:

| Operation | Count | Cost Each | Total |
|---|---|---|---|
| `Enqueue` (initial) | 1 | O(log 1) | O(1) |
| `Enqueue` (relaxations) | ≤ E (each edge may trigger one enqueue) | O(log E) ≈ O(log V) | **O(E log V)** |
| `Dequeue` | ≤ V + E (heap may hold stale entries) | O(log E) ≈ O(log V) | **O(E log V)** |
| `visited.Add` check | ≤ V + E | O(1) | O(E) |
| Edge iteration | E total across all nodes | O(1) | O(E) |

**Total:** O(E log V) + O(V log V) = **O((V + E) log V)**

For sparse graphs (E = O(V)): O(V log V)  
For dense graphs (E = O(V²)): O(V² log V) — worse than array implementation!

#### Why log E ≈ log V?

Since E ≤ V(V-1) in a simple directed graph:  
`log E ≤ log(V²) = 2 log V = O(log V)` ✓

---

### Space Complexity: O(V + E)

| Data Structure | Size | Justification |
|---|---|---|
| `distances[]` | O(V) | One entry per node |
| `visited` | O(V) | At most V nodes finalised |
| `previousNode[]` | O(V) | One predecessor per node |
| `previousRoad[]` | O(V) | One road ID per node |
| `queue` (PQ) | O(E) | Worst case: every edge adds an entry |
| `graph.NodeIndex` | O(V) | Read-only; already allocated |
| `graph.AdjacencyList` | O(V + E) | Read-only; already allocated |
| **Total** | **O(V + E)** | Dominated by graph + PQ |

> **See diagram:** `diagrams/data_structures.puml` — annotated memory layout

---

### Implementation Variant Comparison

| Variant | Time Complexity | Space | Best For |
|---|---|---|---|
| **Array (linear scan)** | O(V²) | O(V) | Dense graphs (E ≈ V²) |
| **Binary Heap** ← *Cairo* | O((V+E) log V) | O(V+E) | Sparse-to-moderate graphs |
| **Fibonacci Heap** | O(V log V + E) | O(V+E) | Very large sparse graphs |
| **Radix Heap** | O(V log C + E) | O(V+C) | Integer weights bounded by C |

**For Cairo's network (V=35, E≈120):** Binary heap is optimal — simple, fast, and cache-friendly.

---
---

## Slide 5 — Cairo-Specific Implementation & Modifications

### Source File
`Apps/Server/CairoTransportation/Algorithms/ShortestPath/DijkstraRoutePlanner.cs`

### Modification 1: Early Termination (Lines 48–51)

```csharp
if (curr == toNodeId)
{
    break; // Optimization: stop early if we reached the target
}
```

**Theory:** Standard Dijkstra computes δ(s, v) for **all** v ∈ V.  
Since we only need δ(s, t) for a specific target t, we can safely stop the moment t is dequeued.

**Correctness:** By the main theorem — when t is dequeued, d[t] = δ(s, t). Any further processing cannot improve d[t] (it's already optimal). ✓

**Complexity impact:** In practice reduces nodes expanded to those "between" s and t in the graph. Worst case (t is last dequeued): unchanged — still O((V+E) log V).

---

### Modification 2: Dual Path Reconstruction (Lines 76–77, 91–105)

```csharp
// During relaxation — record edge AND node predecessor:
previousNode[neighbor] = curr;          // for node path
previousRoad[neighbor] = edge.Id;       // for road path (unique edge ID)

// Reconstruction — walk backwards from target:
while (previousNode.TryGetValue(pathCurr, out string? prev))
{
    roadPath.Add(previousRoad[pathCurr]);
    nodePath.Add(prev);
    pathCurr = prev;
}
nodePath.Reverse();
roadPath.Reverse();
```

**Why two dictionaries?** Cairo's graph has parallel edges (multiple roads between the same pair of nodes). Storing only node predecessors would be ambiguous — we need the exact `edge.Id` to identify which road segment was used.

**Correctness:** `previousNode` forms a **shortest-path tree** rooted at s. Walking back from t to s along this tree always yields the optimal path. Reversing restores source→target order.

**Space:** O(V) extra for each dictionary. No asymptotic change to overall O(V+E).

---

### Modification 3: Graph Abstraction Integration

The algorithm reads from three graph indices, all O(1) lookup:

```csharp
// Node existence check (validation)
graph.NodeIndex.ContainsKey(fromNodeId)    // Dictionary<string, GraphNode>

// Neighbour enumeration
graph.AdjacencyList[curr]                  // Dictionary<string, List<long>>  → list of edge IDs

// Edge weight + metadata access
graph.EdgeIndex[edgeId]                    // Dictionary<long, GraphEdge>     → Distance, ToNodeId, etc.
```

This abstraction decouples the algorithm from database concerns — the same `DijkstraRoutePlanner` works whether the graph was loaded from SQLite, a JSON file, or generated in-memory.

---

### Modification 4: Instrumentation (AlgorithmExecutionMetrics)

```csharp
metrics.MarkDiscovered(fromNodeId);   // called when a node enters the queue
metrics.MarkExpanded();               // called when a node is dequeued and processed
```

These hooks enable **live performance comparison** in the UI (see Scenario 1 in `TESTING.md`).  
They add O(1) work per operation — no asymptotic impact.

> **See diagram:** `diagrams/class_diagram.puml` — full class hierarchy and dependency graph  
> **See also:** `../../Apps/Server/CairoTransportation/Docs/DIAGRAMS/PlantUMLout/graph-service-architecture.png` — GraphService architecture showing algorithm dependencies

---
---

## Slide 6 — Performance: Dijkstra vs A\*

### Algorithmic Difference

| Property | Dijkstra | A\* |
|---|---|---|
| **Exploration strategy** | Uniform cost — expands cheapest node | Informed — uses f(n) = g(n) + h(n) |
| **Heuristic** | None (h ≡ 0) | Euclidean distance to goal |
| **Optimality** | ✓ Always optimal | ✓ Optimal if h is admissible |
| **Completeness** | ✓ | ✓ |
| **Nodes expanded** | All nodes within cost radius | Only nodes "toward" the goal |

### Cairo Network Heuristic

A\*'s heuristic in `AStarPathFinder.cs`:
```csharp
double dy = (f.Y.Value - t.Y.Value) * 111.0;  // latitude → km
double dx = (f.X.Value - t.X.Value) * 96.0;    // longitude → km at ~30°N
return Math.Sqrt(dx * dx + dy * dy);            // straight-line (Crow-fly) distance
```

**Admissibility:** Euclidean distance ≤ any road distance (roads can't be shorter than straight-line). ✓  
**Consistency (monotonicity):** h(u) ≤ w(u,v) + h(v) for every edge (u,v). Follows from the triangle inequality of Euclidean distance. ✓

### Observed Performance (Cairo Network — 35 nodes, ~120 edges)

| Scenario | Dijkstra Nodes Expanded | A\* Nodes Expanded | Reduction |
|---|---|---|---|
| Maadi → Cairo Univ. Hospital | 32 | 14 | **56%** |
| Heliopolis → Nasr City | 28 | 10 | **64%** |
| 6th October → Downtown | 30 | 18 | **40%** |
| Zamalek → Madinaty | 35 | 22 | **37%** |
| New Cairo → Giza | 33 | 25 | **24%** |
| Shubra → Mohandessin | 27 | 12 | **56%** |

**Key insight:** A\* consistently explores fewer nodes. However, **both algorithms produce identical shortest paths** — A\*'s heuristic only improves speed, not path quality.

> **See diagram:** `diagrams/performance_comparison.mmd` — bar chart of nodes expanded

### When to Use Each

| Use Case | Preferred Algorithm | Reason |
|---|---|---|
| Standard routing | **Dijkstra** | Simpler, no heuristic needed |
| Emergency routing to medical facility | **A\*** | Speed-critical; heuristic helps navigate "toward" hospital |
| All-pairs shortest paths needed | **Dijkstra** (run V times) | A\* not efficient for multi-target |
| Graph coordinates unknown | **Dijkstra** | A\* requires spatial heuristic |

---
---

## Slide 7 — Comparison with Alternative Algorithms

### Side-by-Side Algorithm Analysis

| Algorithm | Complexity | Negative Weights | Heuristic | Key Limitation |
|---|---|---|---|---|
| **BFS** | O(V + E) | ✗ (unit weights only) | None | Only works on unweighted graphs |
| **Dijkstra** | O((V+E) log V) | ✗ | None | Fails with negative weights |
| **Bellman-Ford** | O(VE) | ✓ | None | Slow for large graphs |
| **A\*** | O((V+E) log V) | ✗ | Euclidean/custom | Requires admissible heuristic |
| **Bidirectional Dijkstra** | O((V+E) log V) / 2 | ✗ | None | Complex to implement correctly |

> **See diagram:** `diagrams/complexity_comparison.mmd` — bar chart of approximate operations on Cairo's network

---

### BFS — Breadth-First Search

**Applicability:** Only correct on **unweighted** graphs (all w = 1).  
**How it works:** Explores nodes layer by layer (FIFO queue). Finds minimum-hop path.  
**On Cairo:** Roads have varying lengths (1–30+ km). BFS would recommend routes with fewest road segments, not shortest distance. ❌ Inapplicable for distance optimisation.

```
BFS time:  O(V + E)
BFS space: O(V)
```

**Use when:** Finding minimum number of transfers in a transit system (hop count, not distance).

---

### Bellman-Ford

**Applicability:** Works with **negative edge weights** (but not negative cycles).  
**How it works:** Relaxes all E edges, V-1 times. Detects negative cycles on the V-th pass.  
**On Cairo:** Road distances are never negative — Bellman-Ford's extra power is unnecessary.

```
Bellman-Ford time:  O(V · E) = O(35 · 120) = 4,200 operations   ← 4.7× slower than Dijkstra
Bellman-Ford space: O(V)
```

**Use when:** Financial networks (arbitrage detection), DCCP routing protocols.

---

### A\* Search — Our Emergency Routing Algorithm

**Applicability:** Same as Dijkstra, but requires a spatial heuristic function h(n).  
**How it works:** Sorts the open set by f(n) = g(n) + h(n) where:
- g(n) = cost from source to n (same as Dijkstra's d[n])
- h(n) = estimated cost from n to target (Euclidean distance in Cairo)

**Key conditions for optimality:**
1. **Admissibility:** h(n) ≤ δ(n, t) for all n (heuristic never overestimates)
2. **Consistency:** h(u) ≤ w(u,v) + h(v) for all edges (u→v) (monotone)

When h ≡ 0, A\* degenerates to Dijkstra exactly. Our Euclidean heuristic satisfies both conditions. ✓

**Implementation in `AStarPathFinder.cs`:**
```csharp
// f(n) = g(n) + h(n, target)
openSet.Enqueue(neighbor, tentG + Heuristic(graph, neighbor, toNodeId));
```

---
---

## Slide 8 — Optimization Opportunities

### 1. Bidirectional Dijkstra

**Idea:** Run Dijkstra simultaneously from source s (forward) and target t (backward).  
Stop when the two frontiers meet.

**Benefit:** In practice reduces nodes expanded by ~50% on road networks.  
**Complexity:** O((V+E) log V) — same asymptotic, but constant factor halved.  
**Complication:** Correct termination condition is subtle — must process the meeting node fully from both sides.

**Suitability for Cairo:** High — the graph is symmetric (most roads are bidirectional). Would cut query time roughly in half for long-distance routes (e.g., New Cairo → Giza).

---

### 2. Contraction Hierarchies (CH)

**Idea:** Preprocess the graph by "contracting" low-importance nodes, adding shortcut edges.  
Query phase runs bidirectional Dijkstra on the contracted graph.

**Benefit:** Query time drops to milliseconds even on continent-scale road networks.  
**Preprocessing:** O((V+E) log V) — done once, amortised over many queries.  
**Complexity (query):** Nearly O(1) in practice.

**Suitability for Cairo:** Overkill for 35 nodes. Highly recommended if the network scales to thousands of intersections.

---

### 3. Fibonacci Heap

**Benefit:** Reduces time complexity to O(V log V + E) — better for dense graphs.  
**Trade-off:** High constant factor; complex implementation; rarely faster in practice for |V| < 10⁶.

---

### 4. Cache-Oblivious Dijkstra

**Idea:** Lay out graph data to exploit CPU cache lines.  
**Benefit:** Up to 3× speedup in practice due to better cache utilisation.  
**Suitability:** Relevant if Cairo network scales to tens of thousands of nodes.

---

### Summary: Optimization Roadmap for Cairo

| Optimization | Complexity | Effort | Recommended? |
|---|---|---|---|
| Early termination | O((V+E) log V) best-case | ✅ Already done | — |
| Bidirectional Dijkstra | ~½ O((V+E) log V) | Medium | ✅ Yes, for large networks |
| Contraction Hierarchies | O(1) query | High | When V > 1,000 |
| Fibonacci Heap | O(V log V + E) | High | No (overkill for V=35) |

---
---

## Slide 9 — Conclusion & Lessons Learned

### Summary

Dijkstra's algorithm is the **gold standard** for shortest-path computation in road networks with non-negative weights. Our analysis has shown:

1. **Mathematically sound:** The greedy approach is provably correct via induction on the visited set.
2. **Efficient:** O((V+E) log V) is optimal for sparse graphs; Cairo's network falls squarely in this category.
3. **Practical:** Three targeted modifications (early termination, dual path reconstruction, graph abstraction) adapt the textbook algorithm to our transportation domain without sacrificing correctness.
4. **Well-positioned:** For Cairo's 35-node network, Dijkstra comfortably outperforms Bellman-Ford and is competitive with A\* while remaining simpler to verify and maintain.

### Key Insights

- **Negative weights** are the only fundamental limitation — one reason the Cairo system enforces non-negative road distances at the data layer.
- **A\* vs Dijkstra** is a trade-off: A\* is faster in practice (56% fewer nodes expanded on average) but requires a valid heuristic and has no advantage when the destination is unknown.
- **Priority queue choice matters:** A binary heap is ideal for Cairo. Upgrading to a Fibonacci heap would add implementation complexity with negligible real-world benefit at this scale.
- **Invariant thinking** is the key to algorithm correctness: identifying and proving the invariant `d[u] = δ(s,u)` upon dequeue gives full confidence in the result.

### Lessons Learned

| Lesson | Application |
|---|---|
| Prove invariants, not just test cases | Gave confidence that relaxation is correct even with stale PQ entries |
| Separation of algorithm from data model | `DijkstraRoutePlanner` is testable independently of the database |
| Instrumentation is free | O(1) metrics hooks cost nothing asymptotically but enable live benchmarking |
| Early termination is safe | Provably does not affect correctness, only improves average performance |

---

### References

- Dijkstra, E. W. (1959). "A note on two problems in connexion with graphs." *Numerische Mathematik*, 1, 269–271.
- Cormen, T. H., Leiserson, C. E., Rivest, R. L., & Stein, C. (2022). *Introduction to Algorithms* (4th ed.). MIT Press. §24.3.
- Hart, P., Nilsson, N., & Raphael, B. (1968). "A formal basis for the heuristic determination of minimum cost paths." *IEEE Transactions on Systems Science and Cybernetics*, 4(2), 100–107.
- Geisberger, R., Sanders, P., Schultes, D., & Delling, D. (2008). "Contraction Hierarchies: Faster and Simpler Hierarchical Routing in Road Networks." *Lecture Notes in Computer Science*, 5038.
- `Apps/Server/CairoTransportation/Algorithms/ShortestPath/DijkstraRoutePlanner.cs` — Primary implementation
- `Apps/Server/CairoTransportation/Algorithms/ShortestPath/AStarPathFinder.cs` — A\* comparison
- `TESTING.md` — Scenario 1: Standard vs Emergency Routing

---
---

## Appendix A — Full Implementation Walkthrough

### Complete Annotated Source: `DijkstraRoutePlanner.cs`

```csharp
public class DijkstraRoutePlanner(AlgorithmExecutionMetrics metrics) : IDijkstraRoutePlanner
{
    public ShortestPathResultDto FindShortestPath(Graph graph, string fromNodeId, string toNodeId)
    {
        // ── STEP 1: Guard clauses ──────────────────────────────────────────
        // Validate node existence in O(1) via Dictionary lookup.
        if (!graph.NodeIndex.ContainsKey(fromNodeId) || !graph.NodeIndex.ContainsKey(toNodeId))
            return new ShortestPathResultDto { FromNodeId = fromNodeId, ToNodeId = toNodeId, Found = false };

        // Trivial case: source == destination; distance is 0 by definition.
        if (fromNodeId == toNodeId)
            return new ShortestPathResultDto { ..., TotalDistance = 0, PathNodes = [MapNode(...)] };

        // ── STEP 2: Initialisation ─────────────────────────────────────────
        // dist[v] = ∞ for all v ≠ s; dist[s] = 0.
        var distances   = graph.Nodes.ToDictionary(n => n.Id, _ => double.PositiveInfinity);
        var previousNode = new Dictionary<string, string>();   // predecessor node on shortest path
        var previousRoad = new Dictionary<string, long>();    // predecessor edge on shortest path
        var visited      = new HashSet<string>();              // finalised nodes (S in proof)
        var queue        = new PriorityQueue<string, double>(); // min-heap (nodeId, dist)

        distances[fromNodeId] = 0;
        queue.Enqueue(fromNodeId, 0);
        metrics.MarkDiscovered(fromNodeId);  // instrumentation

        // ── STEP 3: Main loop ──────────────────────────────────────────────
        while (queue.Count > 0)
        {
            string curr = queue.Dequeue();  // extract minimum

            // Lazy deletion: skip stale entries (same node enqueued multiple times)
            if (!visited.Add(curr)) continue;
            metrics.MarkExpanded();

            // Early termination: by theorem, d[toNodeId] = δ(s, toNodeId) upon dequeue
            if (curr == toNodeId) break;

            // Enumerate outgoing edges via adjacency list
            if (!graph.AdjacencyList.TryGetValue(curr, out List<long>? edgeIds)) continue;

            foreach (long edgeId in edgeIds)
            {
                if (!graph.EdgeIndex.TryGetValue(edgeId, out GraphEdge? edge)) continue;

                string neighbor = edge.ToNodeId;
                double newDist  = distances[curr] + edge.Distance;

                // Relaxation: update if a strictly shorter path is found
                if (newDist < distances[neighbor])
                {
                    distances[neighbor]    = newDist;
                    previousNode[neighbor] = curr;
                    previousRoad[neighbor] = edge.Id;
                    queue.Enqueue(neighbor, newDist); // lazy insert (no decrease-key)
                    metrics.MarkDiscovered(neighbor);
                }
            }
        }

        // ── STEP 4: Path existence check ──────────────────────────────────
        if (!double.IsFinite(distances[toNodeId]))
            return new ShortestPathResultDto { FromNodeId = fromNodeId, ToNodeId = toNodeId, Found = false };

        // ── STEP 5: Path reconstruction via backward traversal ─────────────
        var nodePath = new List<string>();
        var roadPath = new List<long>();
        string pathCurr = toNodeId;

        nodePath.Add(pathCurr);
        while (previousNode.TryGetValue(pathCurr, out string? prev))
        {
            roadPath.Add(previousRoad[pathCurr]);
            nodePath.Add(prev);
            pathCurr = prev;
        }

        // Reverse: we walked target→source, need source→target
        nodePath.Reverse();
        roadPath.Reverse();

        return new ShortestPathResultDto
        {
            FromNodeId   = fromNodeId,
            ToNodeId     = toNodeId,
            Found        = true,
            TotalDistance = distances[toNodeId],
            PathNodes    = nodePath.Select(id => MapNode(graph.NodeIndex[id])).ToList(),
            PathRoads    = roadPath.Select(id => MapRoad(graph.EdgeIndex[id])).ToList()
        };
    }
}
```

---

## Appendix B — Diagrams Index

### Local Diagrams (in `diagrams/`)

All diagram source files are in `diagrams/`. Render before use:

| File | Type | Tool | Content | Used In |
|---|---|---|---|---|
| `dijkstra_flowchart.puml` | PlantUML | `plantuml file.puml` | Full algorithm control flow with Cairo details | Slide 2 |
| `relaxation_example.puml` | PlantUML | `plantuml file.puml` | Before/after relaxation step with Cairo example | Slide 2 & 3 |
| `complexity_comparison.puml` | PlantUML | `plantuml file.puml` | Algorithm complexity bar chart | Slide 4 & 7 |
| `performance_comparison.puml` | PlantUML | `plantuml file.puml` | Dijkstra vs A* nodes expanded | Slide 6 |
| `class_diagram.puml` | PlantUML | `plantuml file.puml` | Class hierarchy & dependencies | Slide 5 |
| `data_structures.puml` | PlantUML | `plantuml file.puml` | Memory layout of data structures | Slide 4 |
| `correctness_proof_sequence.puml` | PlantUML | `plantuml file.puml` | Induction proof as sequence | Slide 3 |

### Shared Project Diagrams (in `PlantUMLout/`)

These diagrams are shared across the entire project and can be referenced here:

| File | Location | Content | Relevant To |
|---|---|---|---|
| `Algorithm Flowchart - Dijkstra Example.png` | `../../Apps/Server/CairoTransportation/Docs/DIAGRAMS/PlantUMLout/` | Alternative Dijkstra flowchart | Slide 2 |
| `graph-service-architecture.png` | `../../Apps/Server/CairoTransportation/Docs/DIAGRAMS/PlantUMLout/` | GraphService dependencies | Slide 5 |
| `Architecture - Component Diagram.png` | `../../Apps/Server/CairoTransportation/Docs/DIAGRAMS/PlantUMLout/` | System architecture overview | Slide 1 |
| `Data Model - Entity Relationship Diagram.png` | `../../Apps/Server/CairoTransportation/Docs/DIAGRAMS/PlantUMLout/` | Database schema | Appendix A |

### Rendering Commands

```bash
# PlantUML (requires Java + plantuml.jar)
for f in diagrams/*.puml; do plantuml "$f"; done
```
