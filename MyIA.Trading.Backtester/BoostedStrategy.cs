using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Accord.MachineLearning;
using Accord.MachineLearning.Boosting;
using Accord.Statistics;
using MyIA.Trading.Backtester;

namespace MyIA.Trading.Backtester
{
    public class BoostedStrategy : ModelStrategy
    {

        public BoostedStrategy(List<BackTestingSettings> configs)
        {
            _Configs = configs;
            var models = _Configs.Select(objBack => objBack.GetModelStrategy(Program.Log).Model).Cast<SvmTradingModel>().Select(objTradingModel=> objTradingModel.Svm).ToList();
            var weights = _Configs.Select(objBack => 1D).ToList();
            Model = new ClassifierTradingModel()
                {Classifier = new MultiClassBoost(weights, models as IList<IClassifier<double[], int>>)};
        }

        private List<BackTestingSettings> _Configs;

        private Dictionary<int, Tuple<int,DateTime>> TargetDates = new  Dictionary<int, Tuple<int, DateTime>>();

        public MultiClassBoost MultiClassBoost => (MultiClassBoost) Model;

        public override int GetResult(TradingTrainingSample objInputs, DateTime time)
        {
            
            var details = MultiClassBoost.DecideDetail(objInputs.Inputs.ToArray());
            for (var index = 0; index < details.Count; index++)
            {
                Tuple<int, DateTime> target;
                if (!TargetDates.TryGetValue(index, out target) || target.Item2 < time)
                {
                    var classified = details[index];
                    var config = _Configs[index];
                    if (classified != 0)
                    {
                        //target = new Tuple<int, DateTime>(classified, time.Add(config.TrainingConfig.OutputPrediction));
                        target = new Tuple<int, DateTime>(classified, time);//.Add(TimeSpan.FromHours(2)));
                        TargetDates[index] = target;
                    }
                }
            }

            var score1 = TargetDates.Count(objPair => objPair.Value.Item1 == 1 && objPair.Value.Item2 >= time);
            var score2 = TargetDates.Count(objPair => objPair.Value.Item1 == 2 && objPair.Value.Item2 >= time);
            //if ((TargetOrder == null || TargetOrder.OrderType == OrderType.Sell) &&  score2 > 4)
            //{
            //    Program.Log($"Date: {time.ToString(CultureInfo.InvariantCulture)} Engaging 2 with score diff: score1 = {score1}, score2 ={score2} ");
            //    return 2;
            //}
            //if ((TargetOrder == null || TargetOrder.OrderType == OrderType.Buy) && score1 > 15 && score2 < 2)
            //{
            //    Program.Log($"Date: {time.ToString(CultureInfo.InvariantCulture)} Engaging 1 with score diff: score1 = {score1}, score2 ={score2} ");
            //    return 1;
            //}

            //if ((TargetOrder == null || TargetOrder.OrderType == OrderType.Sell) && 4*score2 - score1 > 2 )
            //{
            //    Program.Log($"Date: {time.ToString(CultureInfo.InvariantCulture)} Engaging 2 with score diff: score1 = {score1}, score2 ={score2} ");
            //    return 2;
            //}
            //if ((TargetOrder == null || TargetOrder.OrderType == OrderType.Buy) && score1 - 4 * score2 > 2)
            //{
            //    Program.Log($"Date: {time.ToString(CultureInfo.InvariantCulture)} Engaging 1 with score diff: score1 = {score1}, score2 ={score2} ");
            //    return 1;
            //}


            if (time > _TargetReportTime)
            {
                Program.Log($"Date: {time.ToString(CultureInfo.InvariantCulture)} score1 = {score1}, score2 ={score2} ");
                _TargetReportTime.Add(TimeSpan.FromHours(2));
            }


            return 0;


        }

        private DateTime _TargetReportTime = DateTime.MinValue;

    }

   
}