sc.exe delete "Oanda MT5 Data Importer Service"
sc.exe create "Oanda MT5 Data Importer Service" binpath="C:\Users\fatea\Documents\Visual Studio 2022\OandaMT5WorkerService\OandaMT5WorkerService\bin\Release\net8.0\OandaMT5WorkerService.exe"
pause