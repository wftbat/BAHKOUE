using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Aricie;
using MyIA.Trading.Converter;
using Newtonsoft.Json;

namespace MyIA.Trading.Backtester
{

    public class TradingSample
    {
        public virtual Trade TargetTrade { get; set; }

        public virtual List<Trade> Inputs { get; set; } = new List<Trade>();

        public virtual Dictionary<TimeSpan,Trade> Outputs { get; set; } = new Dictionary<TimeSpan,Trade>();

        public virtual Dictionary<decimal, Trade> Peaks { get; set; } = new Dictionary<decimal, Trade>();

        public virtual Dictionary<decimal, Trade> ThresholdPeaks { get; set; } = new Dictionary<decimal, Trade>();


    }
}
