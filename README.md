# SensorMonitor

Система мониторинга датчиков с ML-анализом.

## Требования
- .NET 9 SDK
- Python 3.12
- VS Code

## Запуск

### 1. Python-сервис
cd src/analytics
python -m venv .venv
.venv\Scripts\activate
pip install -r requirements.txt
uvicorn main:app --reload --port 8000

### 2. C# приложение
dotnet run --project src/SensorMonitor.App