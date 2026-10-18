import os
import numpy as np
import joblib
from sklearn.ensemble import RandomForestClassifier
from sklearn.preprocessing import StandardScaler

MODEL_PATH = "model.pkl"
SCALER_PATH = "scaler.pkl"

def _train_dummy_model():
    print("[model] Обучение синтетической модели...")
    np.random.seed(42)
    n = 3000
    normal = np.column_stack([np.random.normal(25, 1.5, n // 2), np.random.normal(50, 3, n // 2), np.random.normal(0.3, 0.05, n // 2)])
    bad = np.column_stack([np.random.normal(32, 3, n // 2), np.random.normal(68, 5, n // 2), np.random.normal(0.8, 0.15, n // 2)])
    X = np.vstack([normal, bad])
    y = np.array([0] * (n // 2) + [1] * (n // 2))
    scaler = StandardScaler().fit(X)
    model = RandomForestClassifier(n_estimators=100, random_state=42)
    model.fit(scaler.transform(X), y)
    joblib.dump(model, MODEL_PATH)
    joblib.dump(scaler, SCALER_PATH)
    print(f"[model] Модель обучена и сохранена в {MODEL_PATH}")
    return model, scaler

def _load_model():
    if os.path.exists(MODEL_PATH) and os.path.exists(SCALER_PATH):
        print(f"[model] Загрузка модели из {MODEL_PATH}")
        return joblib.load(MODEL_PATH), joblib.load(SCALER_PATH)
    return _train_dummy_model()

_model, _scaler = _load_model()

def predict_risk(temperature: float, humidity: float, vibration: float) -> dict:
    x = np.array([[temperature, humidity, vibration]])
    x_scaled = _scaler.transform(x)
    proba = _model.predict_proba(x_scaled)[0]
    risk = float(proba[1])
    if risk < 0.3: status = "ok"
    elif risk < 0.7: status = "warning"
    else: status = "critical"
    return {"risk": risk, "status": status}
