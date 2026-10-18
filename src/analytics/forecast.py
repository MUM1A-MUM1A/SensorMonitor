from typing import List
from schemas import SensorData

def forecast_trend(history: List[SensorData], horizon: int) -> dict:
    if len(history) < 2:
        last_temp = history[-1].temperature if history else 25.0
        forecast = [{"step": i + 1, "temperature": last_temp} for i in range(horizon)]
        return {"forecast": forecast, "confidence": 0.1}
    temps = [h.temperature for h in history]
    trend = (temps[-1] - temps[0]) / (len(temps) - 1)
    last = temps[-1]
    forecast = []
    for i in range(1, horizon + 1):
        predicted = last + trend * i
        forecast.append({"step": i, "temperature": round(predicted, 2)})
    confidence = min(0.9, 0.3 + len(history) * 0.01)
    return {"forecast": forecast, "confidence": round(confidence, 2)}
