using Npgsql;

namespace OandaMT5WorkerService
{
    public class DBFunctions
    {
        private static string GetConnectionString()
        {
            // DB接続文字列の構築
            return $"server=localhost;database=mt5_datas;UID=postgres;password=fateam;port=5432;TimeOut=500; CommandTimeOut=500;ApplicationName=OandaFxTrade;";
        }

        public static NpgsqlConnection ConnectOandaFxServer()
        {
            var connectionString = GetConnectionString();
            var connection = new NpgsqlConnection(connectionString);

            try
            {
                // DB接続に接続
                connection.Open();
            }
            catch
            {
                // 接続の開放を試みる
                connection?.Dispose();
                throw; // エラーを再スロー
            }

            return connection;
        }

        public static IEnumerable<PriceTimeSeries> ReadData(string currencyPair, string timeSpan, int limit)
        {
            // クエリの組み立てを簡潔に
            var tableName = $"data_mt5_import_{timeSpan.ToLower()}";
            var sql = $"SELECT * FROM {tableName} where code_pair='{currencyPair}' ORDER BY mt5_date_time DESC LIMIT {limit}";

            // データリストの初期化
            var priceTimeSeries = new List<PriceTimeSeries>();

            using(var connection = ConnectOandaFxServer())
            using (var command = new NpgsqlCommand(sql, connection))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    // データの読み込みを簡素化
                    var data = new PriceTimeSeries(
                        ReadText(reader, "code_pair"),
                        ReadDateTime(reader, "date_time"),
                        ReadFloat(reader, "open_value"),
                        ReadFloat(reader, "high_value"),
                        ReadFloat(reader, "low_value"),
                        ReadFloat(reader, "close_value"),
                        ReadFloat(reader, "tick_volume"),
                        ReadFloat(reader, "spread_value"),
                        ReadFloat(reader, "real_volume"),
                        ReadDateTime(reader, "mt5_date_time")
                    );

                    priceTimeSeries.Add(data);
                }

                reader.Close();
                connection.Close();
            }

            return priceTimeSeries;
        }

        public static DateTime LastSavedTime(string symbol, string timeSpan)
        {
            var priceTimeSeries = DBFunctions.ReadData(symbol, timeSpan, 1).FirstOrDefault();

            // デフォルトの日付を設定
            var defaultDate = new DateTime(2010, 1, 1);

            // priceTimeSeriesがnullでなければ、そのDateTimeを返す。そうでなければ、defaultDateを返す。
            return priceTimeSeries?.Mt5DateTime.AddSeconds(1) ?? defaultDate;
        }

        private static DateTime ReadDateTime(NpgsqlDataReader reader, string columnName)
        {
            return reader.GetDateTime(reader.GetOrdinal(columnName));
        }

        private static double ReadDouble(NpgsqlDataReader reader, string columnName)
        {
            return reader.GetDouble(reader.GetOrdinal(columnName));
        }

        private static float ReadFloat(NpgsqlDataReader reader, string columnName)
        {
            return reader.GetFloat(reader.GetOrdinal(columnName));
        }

        private static bool ReadBool(NpgsqlDataReader reader, string columnName)
        {
            return reader.GetBoolean(reader.GetOrdinal(columnName));
        }

        private static string ReadText(NpgsqlDataReader reader, string columnName)
        {
            return reader.GetString(reader.GetOrdinal(columnName));
        }
    }
}
