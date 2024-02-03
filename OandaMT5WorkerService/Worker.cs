using System.Diagnostics;

namespace OandaMT5WorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public List<SymbolInformation> symbolInformations = new();

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            // ここにサービス開始時の処理を書く
            symbolInformations.Add(new SymbolInformation("USDJPY", "H1", DBFunctions.LastSavedTime("USDJPY", "H1")));
            symbolInformations.Add(new SymbolInformation("EURUSD", "H1", DBFunctions.LastSavedTime("EURUSD", "H1")));

            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                for (int i=0;i<symbolInformations.Count;i++)
                {
                    ProcessStartInfo psInfo = new ProcessStartInfo
                    {
                        WorkingDirectory = $"h:\\MT5Python",
                        FileName = "python3.9.exe",
                        CreateNoWindow = false, // コンソール・ウィンドウを開かない
                        UseShellExecute = false, // シェル機能を使用しない
                        RedirectStandardOutput = true // 標準出力をリダイレクト
                    };

                    psInfo.ArgumentList.Add($"H:\\MT5Python\\MT5DataImportStartUp.py");
                    psInfo.ArgumentList.Add(symbolInformations[i].SymbolName);
                    psInfo.ArgumentList.Add(symbolInformations[i].Timespan);
                    psInfo.ArgumentList.Add(symbolInformations[i].LastDBSaved.ToString("yyyy-MM-dd HH:mm:ss"));

                    Process? process = Process.Start(psInfo); // アプリの実行開始

                    if (process != null)
                    {
                        string? line;

                        while ((line = process.StandardOutput.ReadLine()) != null)
                        {
                            Console.WriteLine(line);
                        }

                        process.WaitForExit();
                        process.Close();

                        symbolInformations[i].LastDBSaved = DBFunctions.LastSavedTime(symbolInformations[i].SymbolName, symbolInformations[i].Timespan);
                    }
                }

                //5分ごとにデータを取得するためWait
                int waitTime = CalculateWaitTimeToNextInterval(5);
                await Task.Delay(waitTime, stoppingToken);
            }
        }

        /// <summary>
        /// 待ち時間を計算する
        /// </summary>
        /// <param name="intervalMinutes"></param>
        /// <returns></returns>
        static int CalculateWaitTimeToNextInterval(int intervalMinutes)
        {
            DateTime now = DateTime.Now;
            int minutesToNextInterval = intervalMinutes - (now.Minute % intervalMinutes);
            DateTime nextInterval = now.AddMinutes(minutesToNextInterval);
            nextInterval = new DateTime(nextInterval.Year, nextInterval.Month, nextInterval.Day, nextInterval.Hour, nextInterval.Minute, 0);

            return (int)(nextInterval - now).TotalMilliseconds;
        }
    }
}
