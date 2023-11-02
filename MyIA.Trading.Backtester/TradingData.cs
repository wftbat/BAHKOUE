using System;
using System.Collections.Generic;
using System.Linq;
using Accord.Math;

namespace MyIA.Trading.Backtester
{
    //public class TradingData
    //{
    //    public TradingData():this(0)
    //    {
            
    //    }

    //    public TradingData(int rowNb)
    //    {
    //        //Inputs = new List<List<double>>(rowNb);
    //        //Outputs = new List<double>(rowNb);
    //        //Dates = new List<DateTime>(rowNb);
    //        Rows = new List<TradingTrainingSample>(rowNb);
    //    }

    //    //public float[][] Inputs { get; set; } = new float[][] { };

    //    public double[][] GetInputMatrix()
    //    {
    //        //return Inputs.Select(objlist => objlist.ToArray()).ToArray();
    //        return Rows.Select(r => r.Inputs).Select(objlist => objlist.ToArray()).ToArray();
    //    }

    //    public double[] GetOutputValues()
    //    {
    //        //return Outputs.ToArray();
    //        return Rows.Select(r=>r.Output).ToArray();
    //    }

    //    public int[] GetOutputClasses()
    //    {
    //        //return Outputs.Select(obFloat => (int)obFloat).ToArray();
    //        return Rows.Select(r=>r.Output).Select(obFloat => (int)obFloat).ToArray();
    //    }


    //    //public virtual List<List<double>> Inputs { get; set; }

    //    //public virtual List<double> Outputs { get; set; } 

    //    //public virtual List<DateTime> Dates { get; set; } 

    //    //public virtual List<TradingSample> Samples { get; set; } = new List<TradingSample>();

    //    public virtual List<TradingTrainingSample> Rows { get; set; }


    //}

    //public class TradingDataRow
    //{

    //    public virtual List<double> Inputs { get; set; } = new List<double>();

    //    public virtual double Output { get; set; }

    //    public virtual DateTime Date { get; set; }


    //}

}