# Issue: ML-Based Traffic Congestion Prediction

## Category

AI Tools & Technologies (2 Marks)

## Priority

High

## Status

Open

---

## Requirement

Use ML-based traffic prediction (scikit-learn or TensorFlow) trained on the provided temporal traffic data to forecast congestion.

---

## Current State

- `TrafficSignalService` uses a **greedy** algorithm for signal optimization
- `TimeVaryingDijkstra` uses **hardcoded** period multipliers (MORNING: 1.15, EVENING: 1.25, NIGHT: 0.90)
- No ML model exists anywhere in the project

---

## Approach: Jupyter Notebook + Pre-Computed Predictions

**No Python runtime needed in Docker.** The assessor wants to see ML tools & techniques, not a live inference server. We train offline and export predictions as a JSON file that .NET reads at startup.

```
ml/
├── Traffic_Prediction_Analysis.ipynb   ← Full ML pipeline (for assessment)
├── train_model.py                       ← Training script
├── export_predictions.py                ← Generates predictions.json from model
├── traffic_model.joblib                 ← Saved trained model
├── predictions.json                     ← Pre-computed congestion forecasts → copied into .NET
└── requirements.txt                     ← scikit-learn, pandas, numpy, joblib
```

.NET reads `predictions.json` at startup — no Python subprocess, no Flask, no extra Docker service.

---

## Implementation Plan

### 1. Create ML Directory & Export Data

- [ ] Create `ml/` directory at project root
- [ ] Add `requirements.txt` with: scikit-learn, pandas, numpy, joblib, matplotlib, seaborn
- [ ] Export `traffic_flow` + `roads` data from SQLite to `ml/traffic_data.csv`

### 2. Build Jupyter Notebook (`Traffic_Prediction_Analysis.ipynb`)

- [ ] **Section 1: Data Exploration** — Load CSV, show distributions, correlations
- [ ] **Section 2: Feature Engineering**
  - `congestion_ratio` = flow / capacity
  - `period_encoded` (0=morning, 1=evening, 2=night)
  - `road_type` (existing vs potential)
  - `distance`, `condition` as features
- [ ] **Section 3: Model Training**
  - Train **Random Forest Regressor** (handles non-linear relationships)
  - Target: predicted congestion percentage
  - Train/test split: 80/20
  - Also try **Gradient Boosting** for comparison
- [ ] **Section 4: Evaluation**
  - RMSE, R² score, MAE
  - Predictions vs actual scatter plot
  - Feature importance chart
  - Confusion-style heatmap by period
- [ ] **Section 5: Export Predictions**
  - Generate predictions for all road/period combinations
  - Save as `predictions.json`
  - Save model as `traffic_model.joblib`

### 3. Integrate Predictions into .NET Backend

- [ ] Copy `predictions.json` to `Apps/Server/CairoTransportation/Data/predictions.json`
- [ ] Add `CopyToOutputDirectory` for predictions.json in `.csproj`
- [ ] Create `MlPredictionService.cs` that loads predictions.json at startup
- [ ] Use ML-predicted congestion values in `TimeVaryingDijkstraService` instead of hardcoded multipliers
- [ ] Add `GET /api/ml-predictions` endpoint to expose predictions (for demo)

### 4. Docker — No Changes Needed

- [ ] `predictions.json` is copied via existing `COPY . ./` in Dockerfile
- [ ] No Python runtime in Docker image
- [ ] No additional Docker service

---

## Acceptance Criteria

- [ ] Jupyter notebook demonstrates full ML pipeline (explore → train → evaluate → export)
- [ ] Model achieves R² > 0.7 on test set
- [ ] `predictions.json` generated with congestion forecasts per road/period
- [ ] .NET backend reads predictions and uses them in TimeVaryingDijkstra
- [ ] No Python runtime needed in Docker
