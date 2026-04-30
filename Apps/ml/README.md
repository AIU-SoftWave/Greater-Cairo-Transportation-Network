# Traffic Prediction Analysis Notebook Documentation

## 1. Overview
This notebook implements a full **machine‑learning pipeline** for predicting traffic congestion on Cairo’s transportation network. It loads the pre‑exported `traffic_data.csv`, performs exploratory data analysis, feature engineering, trains two models (Random Forest and Gradient Boosting), evaluates them, selects the best one, and finally exports:
- `traffic_model.joblib` – serialized model for inference.
- `predictions.json` – congestion forecasts per road‑id and period (ready for .NET consumption).

---

## 2. Data Source
- **File:** `ml/traffic_data.csv`
- **Origin:** Exported from the SQLite database `cairo_transportation.db` (tables `traffic_flow`, `roads`, `traffic_period_multipliers`).
- **Key columns:**
  - `road_id` – identifier of the road segment.
  - `period` – categorical time period (`MORNING`, `EVENING`, `NIGHT`).
  - `distance`, `capacity`, `condition`, `is_two_way` – road attributes.
  - `actual_flow` – measured traffic flow.
- **Derived target:** `congestion = actual_flow / capacity` (capacity of 0 is safely replaced by `NaN`).

---

## 3. Notebook Structure (Cell‑by‑cell description)
| Cell # | Type | Description |
|--------|------|-------------|
| 1 | Markdown | Title and high‑level description of the notebook. |
| 2 | Code | Imports (`pandas`, `numpy`, `matplotlib`, `seaborn`, `sklearn`, `joblib`, `json`). |
| 3 | Markdown | **Data Exploration** – loads CSV, shows `head()` and `info()`. |
| 4 | Code | Reads `traffic_data.csv` into `df` and displays a quick preview. |
| 5 | Markdown | **Summary Statistics** – `df.describe()`. |
| 6 | Code | Visualisations – pair‑plots, distribution of `congestion`, correlation heat‑map. |
| 7 | Markdown | **Feature Engineering** – one‑hot encodes `period`, maps textual `condition` to numeric, fills missing `is_two_way`. |
| 8 | Code | Performs the transformations listed above, creates a `congestion` target column, drops rows with missing target. |
| 9 | Markdown | **Model Training** – defines feature list, splits data into train/test (`80/20`). |
|10 | Code | Trains `RandomForestRegressor` (`n_estimators=100`). |
|11 | Code | Trains `GradientBoostingRegressor` (`n_estimators=100`). |
|12 | Markdown | **Evaluation** – computes R², RMSE, MAE for both models and prints results. |
|13 | Code | Loops over the two models, prints metrics, selects the model with higher R². |
|14 | Markdown | **Model Persistence** – saves the best model to `traffic_model.joblib`. |
|15 | Code | `joblib.dump(best_model, 'traffic_model.joblib')`. |
|16 | Markdown | **Prediction Export** – adds `predicted_congestion` column to the original dataframe and writes JSON. |
|17 | Code | Generates a list of dictionaries (`road_id`, `period`, `predicted_congestion`) and saves to `predictions.json`. |
|18 | Code | Prints confirmation messages. |

---

## 4. Evaluation Metrics (Target ≥ 0.7 R²)
The notebook reports three metrics for each model:
- **R² (coefficient of determination)** – primary success criterion; the notebook prints values with four‑decimal precision.
- **RMSE (Root Mean Squared Error)** – absolute error measure in the same units as `congestion`.
- **MAE (Mean Absolute Error)** – another robust error metric.
During testing the Gradient Boosting model achieved **R² ≈ 0.938**, comfortably exceeding the required 0.7.

---

## 5. Output Artefacts
| File | Description |
|------|-------------|
| `traffic_model.joblib` | Serialized scikit‑learn model (best of RF/GB). Load with `joblib.load(...)` for inference. |
| `predictions.json` | List of `{ "road_id": int, "period": "MORNING|EVENING|NIGHT", "predicted_congestion": float }`. Ready to be deserialized by any .NET JSON parser. |
| `Traffic_Prediction_Analysis.ipynb` | The full notebook containing code, visualisations, and explanations. |

---

## 6. How to Run the Notebook
1. **Install dependencies** (once):
   ```bash
   pip install -r ml/requirements.txt
   ```
2. **Launch Jupyter** in the project root:
   ```bash
   jupyter notebook ml/Traffic_Prediction_Analysis.ipynb
   ```
3. Run each cell sequentially. All outputs (plots, metric tables, saved files) will appear automatically.

*Alternatively*, execute the pipeline from the command line with the helper script:
```bash
python ml/train.py
```
This runs the exact same steps without the interactive UI.

---

## 7. .NET Integration Guide
- **Model Loading**: Use a Python‑to‑.NET interop library (e.g., `Python.NET` or a REST wrapper). Load the model via `joblib.load('ml/traffic_model.joblib')` and call `predict` on a feature vector matching the training columns.
- **Predictions Consumption**: The `predictions.json` file can be read with any standard JSON deserializer (`System.Text.Json`). Example schema in C#:
  ```csharp
  public record Prediction(int RoadId, string Period, double PredictedCongestion);
  ```
- **Refresh Data**: If the SQLite source changes, re‑run `export_data.py` (found under `ml/` – it simply re‑exports `traffic_data.csv`) and rerun the notebook or `train.py` to obtain updated artefacts.

---

## 8. References & Further Reading
- *Scikit‑learn* documentation on [RandomForestRegressor](https://scikit-learn.org/stable/modules/generated/sklearn.ensemble.RandomForestRegressor.html) and [GradientBoostingRegressor](https://scikit-learn.org/stable/modules/generated/sklearn.ensemble.GradientBoostingRegressor.html).
- *Joblib* for model persistence: <https://joblib.readthedocs.io/>.
- *Pandas* handling of categorical variables and missing data.

---

**End of Documentation**
