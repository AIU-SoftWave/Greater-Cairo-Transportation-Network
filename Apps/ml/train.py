import pandas as pd
import numpy as np
from sklearn.model_selection import train_test_split
from sklearn.ensemble import RandomForestRegressor, GradientBoostingRegressor
from sklearn.metrics import mean_squared_error, r2_score, mean_absolute_error
import joblib
import json

def train_and_evaluate():
    # Load data
    df = pd.read_csv('traffic_data.csv')
    
    # Feature engineering
    # period is categorical: MORNING, EVENING, NIGHT
    df = pd.get_dummies(df, columns=['period'], drop_first=False)
    
    # condition is string (e.g. 'GOOD', 'FAIR', 'POOR'), encode if exists
    if 'condition' in df.columns and df['condition'].dtype == object:
        condition_map = {'EXCELLENT': 4, 'GOOD': 3, 'FAIR': 2, 'POOR': 1}
        df['condition_num'] = df['condition'].map(condition_map).fillna(2) # Default to FAIR
    else:
        df['condition_num'] = df['condition'] if 'condition' in df.columns else 2
        
    df['is_two_way'] = df['is_two_way'].fillna(1).astype(int)
    
    # We want to predict traffic flow (actually, congestion = flow / capacity)
    # Let's predict flow directly, then congestion can be calculated, or predict congestion directly.
    # The requirement says "traffic congestion prediction". So let's predict congestion = flow / capacity
    df['congestion'] = df['actual_flow'] / df['capacity'].replace(0, np.nan)
    df = df.dropna(subset=['congestion'])
    
    features = ['distance', 'capacity', 'condition_num', 'is_two_way']
    for p in ['period_MORNING', 'period_EVENING', 'period_NIGHT']:
        if p in df.columns:
            features.append(p)
            
    X = df[features]
    y = df['congestion']
    
    X_train, X_test, y_train, y_test = train_test_split(X, y, test_size=0.2, random_state=42)
    
    # Train Random Forest
    rf = RandomForestRegressor(n_estimators=100, random_state=42)
    rf.fit(X_train, y_train)
    
    # Train Gradient Boosting
    gb = GradientBoostingRegressor(n_estimators=100, random_state=42)
    gb.fit(X_train, y_train)
    
    # Evaluate
    for name, model in [('Random Forest', rf), ('Gradient Boosting', gb)]:
        y_pred = model.predict(X_test)
        r2 = r2_score(y_test, y_pred)
        rmse = np.sqrt(mean_squared_error(y_test, y_pred))
        mae = mean_absolute_error(y_test, y_pred)
        print(f"--- {name} ---")
        print(f"R2: {r2:.4f}, RMSE: {rmse:.4f}, MAE: {mae:.4f}")
        
    # Choose best model (e.g. Random Forest)
    best_model = rf if r2_score(y_test, rf.predict(X_test)) > r2_score(y_test, gb.predict(X_test)) else gb
    print(f"Best model chosen: {type(best_model).__name__}")
    
    # Save model
    joblib.dump(best_model, 'traffic_model.joblib')
    
    # Export predictions
    df['predicted_congestion'] = best_model.predict(X)
    
    # Re-map period from dummies if needed, or just save as requested
    # We'll reload original df to match road_id and period
    df_orig = pd.read_csv('traffic_data.csv')
    df_orig['predicted_congestion'] = df['predicted_congestion']
    
    predictions = []
    for _, row in df_orig.iterrows():
        predictions.append({
            'road_id': int(row['road_id']),
            'period': str(row['period']),
            'predicted_congestion': float(row['predicted_congestion']) if not pd.isna(row['predicted_congestion']) else 0.0
        })
        
    with open('predictions.json', 'w') as f:
        json.dump(predictions, f, indent=4)
        
    print("Saved predictions.json")

if __name__ == '__main__':
    train_and_evaluate()
