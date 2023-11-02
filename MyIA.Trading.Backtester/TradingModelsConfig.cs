using System;
using Accord.MachineLearning;

namespace MyIA.Trading.Backtester
{

    public enum TradingModelType
    {
        MulticlassSvm,
        AutoML
    }

    public class TradingModelsConfig
    {

        public TradingModelType ModelType { get; set; }

        public TradingModelConfig CurrentModelConfig
        {
            get
            {
                switch (ModelType)
                {
                    case TradingModelType.MulticlassSvm:
                        return SvmModelConfig;
                    case TradingModelType.AutoML:
                        return AutomMlModelConfig;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        } 

        public TradingSvmModelConfig SvmModelConfig { get; set; } = new TradingSvmModelConfig();

        public TradingAutoMlModelConfig AutomMlModelConfig { get; set; } = new TradingAutoMlModelConfig();



    }
}