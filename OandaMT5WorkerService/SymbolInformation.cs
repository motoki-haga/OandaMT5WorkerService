using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OandaMT5WorkerService
{
    public class SymbolInformation
    {
        public string SymbolName { get; set; } = string.Empty;
        public string Timespan { get; set; } = string.Empty;
        public DateTime LastDBSaved { get; set; }=new DateTime(2010,1,1);

        public SymbolInformation(string symbolName, string timespan, DateTime lastDBSaved)
        {
            SymbolName = symbolName;
            Timespan = timespan;
            LastDBSaved = lastDBSaved;
        }
    }
}
