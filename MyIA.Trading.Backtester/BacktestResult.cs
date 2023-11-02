using System;
using FileHelpers;

namespace MyIA.Trading.Backtester
{
    [DelimitedRecord(";")]
    public class BacktestResult
    {

        public string BackTestPeriod { get; set; }
        public string ModelName { get; set; }

        public  double TestError { get; set; }
        public decimal Result { get; set; }
        public int TradeNb { get; set; }

        public string Trade1 { get; set; }
        public string Trade2 { get; set; }
        public string Trade3 { get; set; }
        public string Trade4 { get; set; }
        public string Trade5 { get; set; }
        public string Trade6 { get; set; }
        public string Trade7 { get; set; }
        public string Trade8 { get; set; }
        public string Trade9 { get; set; }
        public string Trade10 { get; set; }
        public string Trade11 { get; set; }
        public string Trade12 { get; set; }
        public string Trade13 { get; set; }
        public string Trade14 { get; set; }

        public string Trade15 { get; set; }

        public string Trade16 { get; set; }
        public string Trade17 { get; set; }
        public string Trade18 { get; set; }
        public string Trade19 { get; set; }
        public string Trade20 { get; set; }
        public string Trade21 { get; set; }
        public string Trade22 { get; set; }
        public string Trade23 { get; set; }
        public string Trade24 { get; set; }
        public string Trade25 { get; set; }
        public string Trade26 { get; set; }
        public string Trade27 { get; set; }
        public string Trade28 { get; set; }
        public string Trade29 { get; set; }
        public string Trade30 { get; set; }




    }
}
