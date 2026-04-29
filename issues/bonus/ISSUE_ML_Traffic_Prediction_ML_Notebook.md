# Sub-Issue: ML Pipeline & Notebook for Traffic Prediction

## Category
AI Tools & Technologies (ML Pipeline)

## Priority
High

## Status
Open

---

## Requirement
Develop a Jupyter notebook and supporting scripts to train an ML model (Random Forest/Gradient Boosting) for traffic congestion prediction using provided data. Export predictions as a JSON file for .NET integration.

---

## Implementation Plan
- [ ] Create `ml/` directory at project root
- [ ] Add `requirements.txt` with: scikit-learn, pandas, numpy, joblib, matplotlib, seaborn
- [ ] Export `traffic_flow` + `roads` data from SQLite to `ml/traffic_data.csv`
- [ ] Build Jupyter Notebook (`Traffic_Prediction_Analysis.ipynb`) with:
    - Data exploration
    - Feature engineering
    - Model training (Random Forest, Gradient Boosting)
    - Evaluation (RMSE, R², MAE, plots)
    - Export predictions to `predictions.json` and model to `traffic_model.joblib`
- [ ] Save outputs in `ml/` directory

---

## Acceptance Criteria
- [ ] Jupyter notebook demonstrates full ML pipeline
- [ ] Model achieves R² > 0.7 on test set
- [ ] `predictions.json` generated with congestion forecasts per road/period
- [ ] Outputs are ready for .NET integration

---

## Blocks
- [Integrate ML Predictions with .NET Backend](ISSUE_ML_Traffic_Prediction_DotNet_Integration.md)
