using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QConsoleWeb.Models
{
    public class DataChartJs
    {
        public List<string> labels { get; set; }
        public List<SerieChartJs> datasets { get; set; }
    }

    public class SerieChartJs
    {
        public List<int> data { get; set; }
        public string label { get; set; }
        public string borderColor { get; set; }
        public string pointStyle { get; set; }
        public double pointRadius { get; set; }
        public string backgroundColor { get; set; }
        public double borderWidth { get; set; }
        
    }
}
