/*
 * QUANTCONNECT.COM - Democratizing Finance, Empowering Individuals.
 * Lean Algorithmic Trading Engine v2.0. Copyright 2014 QuantConnect Corporation.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
*/

using System.Collections.Generic;
using QuantConnect.Data.Market;
using QuantConnect.Indicators;
using QuantConnect.Parameters;
using QuantConnect.Interfaces;
using QuantConnect.Brokerages;

namespace QuantConnect.Algorithm.CSharp
{
    /// <summary>
    /// Demonstration of the parameter system of QuantConnect. Using parameters you can pass the values required into C# algorithms for optimization.
    /// </summary>
    /// <meta name="tag" content="optimization" />
    /// <meta name="tag" content="using quantconnect" />
    public class JsboigeParameterizedAlgorithm : QCAlgorithm, IRegressionAlgorithmDefinition
    {
        // We place attributes on top of our fields or properties that should receive
        // their values from the job. The values 100 and 200 are just default values that
        // are only used if the parameters do not exist.
        [Parameter("ema-fast")]
        public int FastPeriod = 100;

        [Parameter("ema-slow")]
        public int SlowPeriod = 150;

        public ExponentialMovingAverage Fast;
        public ExponentialMovingAverage Slow;

        private Symbol _btcusd;

        //private string _ChartName = "Trade Plot";
        //private string _PriceSeriesName = "Price";
        //private string _PortfoliovalueSeriesName = "PortFolioValue";


        public override void Initialize()
        {

            SetStartDate(2017, 11, 25); // début backtest
            SetEndDate(2020, 05, 1); // fin backtest

            //SetStartDate(2021, 1, 1); // début backtest
            //SetEndDate(2023, 03, 22); // fin backtest

            this.SetWarmUp(150);

            SetBrokerageModel(BrokerageName.Bitstamp, AccountType.Cash);

            SetCash(10000); // capital

            _btcusd = AddCrypto("BTCUSD", Resolution.Daily).Symbol;

            Fast = EMA(_btcusd, FastPeriod);
            Slow = EMA(_btcusd, SlowPeriod);
        }

        public void OnData(TradeBars data)
        {

            // wait for our indicators to ready
            if (this.IsWarmingUp || !Fast.IsReady || !Slow.IsReady) return;

            if (!Portfolio.Invested && Fast > Slow*1.001m)
            {
                SetHoldings(_btcusd, 1);
            }
            else if (Portfolio.Invested && Fast < Slow*0.999m)
            {
                Liquidate(_btcusd);
            }
        }

        /// <summary>
        /// This is used by the regression test system to indicate if the open source Lean repository has the required data to run this algorithm.
        /// </summary>
        public bool CanRunLocally { get; } = true;

        /// <summary>
        /// This is used by the regression test system to indicate which languages this algorithm is written in.
        /// </summary>
        public Language[] Languages { get; } = { Language.CSharp, Language.Python };

        /// <summary>
        /// Data Points count of all timeslices of algorithm
        /// </summary>
        public long DataPoints => 3943;

        /// <summary>
        /// Data Points count of the algorithm history
        /// </summary>
        public int AlgorithmHistoryDataPoints => 0;

        /// <summary>
        /// This is used by the regression test system to indicate what the expected statistics are from running the algorithm
        /// </summary>
        public Dictionary<string, string> ExpectedStatistics => new Dictionary<string, string>
        {
            {"Total Trades", "1"},
            {"Average Win", "0%"},
            {"Average Loss", "0%"},
            {"Compounding Annual Return", "286.047%"},
            {"Drawdown", "0.300%"},
            {"Expectancy", "0"},
            {"Net Profit", "1.742%"},
            {"Sharpe Ratio", "23.023"},
            {"Probabilistic Sharpe Ratio", "0%"},
            {"Loss Rate", "0%"},
            {"Win Rate", "0%"},
            {"Profit-Loss Ratio", "0"},
            {"Alpha", "1.266"},
            {"Beta", "0.356"},
            {"Annual Standard Deviation", "0.086"},
            {"Annual Variance", "0.007"},
            {"Information Ratio", "-0.044"},
            {"Tracking Error", "0.147"},
            {"Treynor Ratio", "5.531"},
            {"Total Fees", "$3.45"},
            {"Estimated Strategy Capacity", "$48000000.00"},
            {"Lowest Capacity Asset", "SPY R735QTJ8XC9X"},
            {"Portfolio Turnover", "19.72%"},
            {"OrderListHash", "d54f031ece393c8b3fc653ca3e6259f8"}
        };
    }
}
