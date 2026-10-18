from pydantic import BaseModel, Field
from typing import List

class SensorData(BaseModel):
    temperature: float = Field(..., description="Температура")
    humidity: float = Field(..., description="Влажность")
    vibration: float = Field(..., description="Вибрация")

class PredictionResponse(BaseModel):
    risk: float = Field(..., ge=0.0, le=1.0)
    status: str = Field(...)

class ForecastPoint(BaseModel):
    step: int
    temperature: float

class ForecastRequest(BaseModel):
    history: List[SensorData] = Field(..., min_length=1)
    horizon: int = Field(default=10, ge=1, le=100)

class ForecastResponse(BaseModel):
    forecast: List[ForecastPoint]
    confidence: float = Field(..., ge=0.0, le=1.0)
