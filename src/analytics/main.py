from fastapi import FastAPI
from fastapi.middleware.cors import CORSMiddleware
from schemas import SensorData, PredictionResponse, ForecastRequest, ForecastResponse
from model import predict_risk
from forecast import forecast_trend

app = FastAPI(title="Sensor Analytics", version="0.1.0")

app.add_middleware(CORSMiddleware, allow_origins=["*"], allow_methods=["*"], allow_headers=["*"])

@app.get("/health")
def health(): return {"status": "ok"}

@app.post("/predict", response_model=PredictionResponse)
def predict(data: SensorData):
    return predict_risk(temperature=data.temperature, humidity=data.humidity, vibration=data.vibration)

@app.post("/forecast", response_model=ForecastResponse)
def forecast(request: ForecastRequest):
    return forecast_trend(request.history, request.horizon)
