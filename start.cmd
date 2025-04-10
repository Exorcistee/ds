@echo off
cd nats-server\
start nats-server

cd ..\RankCalculator\
start dotnet run

cd ..\Valuator\
start dotnet run --urls "http://0.0.0.0:5001"
start dotnet run --urls "http://0.0.0.0:5002"

cd ..\nginx\
start nginx