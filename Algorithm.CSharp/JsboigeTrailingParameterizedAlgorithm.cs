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

using System;
using System.Collections.Generic;
using QuantConnect.Data.Market;
using QuantConnect.Indicators;
using QuantConnect.Parameters;
using QuantConnect.Interfaces;
using QuantConnect.Brokerages;
using QuantConnect.Data;
using QuantConnect.Orders.Fees;
using QuantConnect.Securities;
using QuantConnect.Algorithm.Framework.Risk;
using QuantConnect.Algorithm.Framework.Execution;
using QuantConnect.Orders;

namespace QuantConnect.Algorithm.CSharp
{
    /// <summary>
    /// Demonstration of the parameter system of QuantConnect. Using parameters you can pass the values required into C# algorithms for optimization.
    /// </summary>
    /// <meta name="tag" content="optimization" />
    /// <meta name="tag" content="using quantconnect" />
    public class JsboigeTrailingParameterizedAlgorithm : QCAlgorithm, IRegressionAlgorithmDefinition
    {
        // We place attributes on top of our fields or properties that should receive
        // their values from the job. The values 100 and 200 are just default values that
        // are only used if the parameters do not exist.
        [Parameter("ema-fast")]
        public int FastPeriod = 18;

        [Parameter("ema-slow")]
        public int SlowPeriod = 21;

        public ExponentialMovingAverage Fast;
        public ExponentialMovingAverage Slow;

        private Symbol _btcusd;

        //private string _ChartName = "Trade Plot";
        //private string _PriceSeriesName = "Price";
        //private string _PortfoliovalueSeriesName = "PortFolioValue";


        public override void Initialize()
        {

            SetStartDate(2017, 12, 15); // début backtest
            SetEndDate(2022, 11, 20); // fin backtest

            //SetStartDate(2017, 11, 25); // début backtest
            //SetEndDate(2020, 05, 1); // fin backtest

            //SetStartDate(2021, 1, 1); // début backtest
            //SetEndDate(2023, 03, 22); // fin backtest

            this.SetWarmUp(TimeSpan.FromDays(150));

            //SetBenchmark(x => 0);

            SetBrokerageModel(BrokerageName.Bitstamp, AccountType.Cash);
            var btcSecurity = AddCrypto("BTCUSD", Resolution.Daily);
            _btcusd = btcSecurity.Symbol;


            
            Fast = this.EMA(_btcusd, FastPeriod, Resolution.Daily);
            Slow = this.EMA(_btcusd, SlowPeriod, Resolution.Daily);
            //this.AddRiskManagement(new TrailingStopRiskManagementModel(_StopDrawdown));
            //SetExecution(new VolumeWeightedAveragePriceExecutionModel());

        }

        private decimal _StopDrawdown = 0.2m;
        private DateTime _StopDate = DateTime.MinValue;
        private decimal _StopPrice = Decimal.MaxValue;
        
        public override void OnData(Slice data)
        {

            // wait for our indicators to ready
            if (this.IsWarmingUp || !Fast.IsReady || !Slow.IsReady) return;

           
            if (!Portfolio.Invested && (Fast > Slow*1.001m) && (data.UtcTime>_StopDate || data.Bars[_btcusd].Close> _StopPrice))
            {
                SetHoldings(_btcusd, 1);
                _StopDate = DateTime.MinValue;
                _StopPrice = Decimal.MaxValue;
            }
            else if (Portfolio.Invested && Fast < Slow*0.999m)
            {
                Liquidate(_btcusd);
                
            }
            
        }

        public override void OnOrderEvent(OrderEvent orderEvent)
        {
           
            if (orderEvent.Status == OrderStatus.Filled )
            {

                string message = "";
                if (orderEvent.Quantity < 0)
                {
                    message = "Sold";
                    if (Fast > Slow)
                    {
                        message = $"Stop {message}";
                        //_StopDate = orderEvent.UtcTime.AddDays(FastPeriod);
                        //_StopPrice = orderEvent.FillPrice / (1 - _StopDrawdown);
                    }
                }
                else
                {
                    message = "Purchased";
                }

                var endMessage =
                    $"Time: {orderEvent.UtcTime.ToShortDateString()}, Price:  @{this.CurrentSlice.Bars[_btcusd].Close}$/Btc; Portfolio: {Portfolio.CashBook[Portfolio.CashBook.AccountCurrency].Amount}$, {Portfolio[_btcusd].Quantity}BTCs, Total Value: {Portfolio.TotalPortfolioValue}$, Total Fees: {Portfolio.TotalFees}$";
                Debug($"{message} {endMessage}");

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
