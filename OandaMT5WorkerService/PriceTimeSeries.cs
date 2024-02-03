using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace OandaMT5WorkerService
{
    public class PriceTimeSeries
    {
        public string CodePair { get; set; } = string.Empty;
        public DateTime DateTime { get; set; } = new DateTime(1900, 1, 1);
        public float OpenValue { get; set; } = 0;
        public float HighValue { get; set; } = 0;
        public float LowValue { get; set; } = 0;
        public float CloseValue { get; set; } = 0;
        public float TickVolume { get; set; } = 0;
        public float SpreadValue { get; set; } = 0;
        public float RealVolume { get; set; } = 0;
        public DateTime Mt5DateTime { get; set; } = new DateTime(1900, 1, 1);

        public PriceTimeSeries()
        { }

        public PriceTimeSeries(string codePair, DateTime dateTime, float openValue, float highValue, float lowValue, float closeValue, float tickVolume, float spreadValue, float realVolume, DateTime mt5DateTime)
        {
            CodePair = codePair;
            DateTime = dateTime;
            OpenValue = openValue;
            HighValue = highValue;
            LowValue = lowValue;
            CloseValue = closeValue;
            TickVolume = tickVolume;
            SpreadValue = spreadValue;
            RealVolume = realVolume;
            Mt5DateTime = mt5DateTime;
        }
    }
}
