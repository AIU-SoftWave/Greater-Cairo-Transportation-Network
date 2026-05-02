# Traffic Control Module

## Purpose

Handles traffic flow lookup, traffic policy multipliers, signal optimization, and ML-based congestion prediction.

## Controllers and Base Routes

- `TrafficController` -> `GET /api/traffic-monitoring/*`
- `TrafficPeriodMultipliersController` -> `GET /api/traffic-policy/*`
- `TrafficSignalController` -> `GET /api/signal-optimization`
- `MlPredictionsController` -> `GET /api/ml-predictions`

## Endpoints

- `GET /api/traffic-monitoring/road/{roadId}` - Get traffic flow by road ID
- `GET /api/traffic-monitoring/period/{period}` - Get traffic flow by period (MORNING/EVENING/NIGHT)
- `GET /api/traffic-policy` - Get all period multipliers
- `GET /api/traffic-policy/{period}` - Get multiplier for specific period
- `GET /api/signal-optimization?period=MORNING&topN=10&analyzeAllIntersections=false` - Optimize traffic signals
- `GET /api/ml-predictions` - Get all ML-predicted congestion values
- `GET /api/ml-predictions/{roadId}/{period}` - Get prediction for specific road and period

## Services

- `TrafficService` - Traffic flow data management
- `TrafficSignalService` - Signal optimization logic
- `MlPredictionService` - ML model predictions for road congestion

## ML Predictions

The system includes pre-computed ML predictions from a Gradient Boosting model (R² = 0.94) trained on historical traffic data. Predictions are stored in `Data/predictions.json` and include:

- `road_id`: Road identifier
- `period`: Time period (MORNING/EVENING/NIGHT)
- `predicted_congestion`: Normalized congestion value (0-2 scale)

These predictions are used by the Time-Varying routing algorithm to make smarter routing decisions.
