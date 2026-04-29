# Sub-Issue: Integrate ML Predictions with .NET Backend

## Category
Backend Integration

## Priority
High

## Status
Blocked (by ML Pipeline & Notebook)

---

## Requirement
Integrate the exported ML predictions (`predictions.json`) into the .NET backend, replacing hardcoded congestion multipliers with model-driven values. Expose predictions via an API endpoint for demo purposes.

---

## Implementation Plan
- [ ] Copy `predictions.json` to `Apps/Server/CairoTransportation/Data/predictions.json`
- [ ] Add `CopyToOutputDirectory` for predictions.json in `.csproj`
- [ ] Create `MlPredictionService.cs` to load predictions at startup
- [ ] Use ML-predicted congestion values in `TimeVaryingDijkstraService`
- [ ] Add `GET /api/ml-predictions` endpoint

---

## Acceptance Criteria
- [ ] .NET backend reads and uses ML predictions
- [ ] API endpoint exposes predictions
- [ ] No Python runtime required in Docker

---

## Blocked By
- [ML Pipeline & Notebook for Traffic Prediction](ISSUE_ML_Traffic_Prediction_ML_Notebook.md)
