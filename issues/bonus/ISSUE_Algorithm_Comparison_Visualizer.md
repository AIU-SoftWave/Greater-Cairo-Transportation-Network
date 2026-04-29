# Issue: Side-by-Side Algorithm Comparison Visualizer

## Category
Enhanced Visualization & UI (2 Marks)

## Priority
High

## Status
Open

---

## Requirement
Side-by-side algorithm comparison visualizer (e.g., Dijkstra vs A* race animation).

---

## Current State
- Single algorithm view — select one algorithm at a time and see its result
- No comparison mode exists
- `MapView.tsx` has algorithm selection buttons but only one active at a time
- Backend returns `trace` with `visitedNodes`, `expandedNodes`, `executionTimeMs` for each algorithm

---

## Implementation Plan

### 1. Add "Compare" Algorithm Mode
- [ ] Add `"compare"` to `AlgorithmType` in `MapView.tsx`
- [ ] Add compare button to algorithm selection panel
- [ ] When compare mode is active, show two algorithm dropdowns (left/right)

### 2. Dual Algorithm Selection UI
- [ ] Add state: `compareAlgoA`, `compareAlgoB` (default: dijkstra, astar)
- [ ] Add two dropdown selectors in the dashboard when compare mode is active
- [ ] Both use the same start/end nodes

### 3. Dual API Calls & Results
- [ ] When start & end are set in compare mode, fire both API calls simultaneously
- [ ] Store both responses: `compareResponseA`, `compareResponseB`
- [ ] Show both paths on map with distinct colors:
  - Algorithm A: blue (`#3b82f6`)
  - Algorithm B: green (`#22c55e`)

### 4. Side-by-Side Metrics Panel
- [ ] Create `CompareResultsPanel` component showing:
  - Algorithm name (left vs right)
  - Execution time (ms)
  - Nodes visited / expanded
  - Total distance (km)
  - Estimated travel time (min)
  - Path roads count
- [ ] Highlight the "winner" for each metric (green background)

### 5. Race Animation (Bonus)
- [ ] Animate path drawing: progressively reveal path nodes with `setTimeout`
- [ ] Show explored nodes expanding outward for each algorithm
- [ ] Use different colored circles for each algorithm's frontier

### 6. Predefined Comparisons
- [ ] Add quick-select buttons for common comparisons:
  - "Dijkstra vs A*" (shortest path comparison)
  - "Dijkstra vs Time-Varying" (traffic impact)
  - "A* vs Time-Varying" (emergency + traffic)

---

## Files to Modify
- `Apps/client/src/components/MapView.tsx` — main component
- `Apps/client/src/components/CompareResultsPanel.tsx` — new component
- `Apps/client/src/types/index.ts` — add compare types if needed

---

## Acceptance Criteria
- [ ] Compare mode shows two algorithm selectors
- [ ] Both paths rendered on map with distinct colors
- [ ] Side-by-side metrics panel with all key indicators
- [ ] Works for Dijkstra vs A* and Dijkstra vs Time-Varying comparisons
