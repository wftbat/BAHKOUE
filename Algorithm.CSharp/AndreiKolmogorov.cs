using QuantConnect.Algorithm;
using QuantConnect.Brokerages;
using QuantConnect.Data;
using QuantConnect.Indicators;
using QuantConnect.Data.Market;
using System;
using System.Drawing;

namespace QuantConnect
{
    public class AndreiKolmogorov : QCAlgorithm
    {
        private MovingAverageConvergenceDivergence _macd;
        private Symbol _btcusd;
        private const decimal _tolerance = 0.0025m;
        private bool _invested;

        private string _ChartName = "Trade Plot";
        private string _PriceSeriesName = "Price";
        private string _PortfoliovalueSeriesName = "PortFolioValue";

        public override void Initialize()
        {
            SetStartDate(2020, 1, 1); // début backtest
            SetEndDate(2023, 10, 31); // fin backtest


            SetBrokerageModel(BrokerageName.Bitstamp, AccountType.Cash);

            SetCash(10000); // capital

            _btcusd = AddCrypto("BTCUSD", Resolution.Daily).Symbol;


            _macd = MACD(_btcusd, 12, 26, 9, MovingAverageType.Exponential, Resolution.Daily, Field.Close);

            
            // Dealing with plots
            var stockPlot = new Chart(_ChartName);
            var assetPrice = new Series(_PriceSeriesName, SeriesType.Line, "$", Color.Blue);
            var portFolioValue = new Series(_PortfoliovalueSeriesName, SeriesType.Line, "$", Color.Green);
            stockPlot.AddSeries(assetPrice);
            stockPlot.AddSeries(portFolioValue);
            AddChart(stockPlot);
            Schedule.On(DateRules.EveryDay(), TimeRules.Every(TimeSpan.FromDays(1)), DoPlots);

        }

        private void DoPlots()
        {
            Plot(_ChartName, _PriceSeriesName, Securities[_btcusd].Price);
            Plot(_ChartName, _PortfoliovalueSeriesName, Portfolio.TotalPortfolioValue);
        }

        public override void OnData(Slice data)
        {
            if (!_macd.IsReady) return;

            var holdings = Portfolio[_btcusd].Quantity;

            if (holdings <= 0 && _macd > _macd.Signal * (1 + _tolerance)) // condition d'achat
            {
                SetHoldings(_btcusd, 1.0);

                Debug($"Purchased BTC @{data.Bars[_btcusd].Close}$/Btc; Portfolio: {Portfolio.Cash}$, {Portfolio[_btcusd].Quantity}BTCs, Total Value: {Portfolio.TotalPortfolioValue}$, Total Fees: {Portfolio.TotalFees}$");
                _invested = true;
            }
            else if (_invested && _macd < _macd.Signal) // condition de vente
            {
                Liquidate(_btcusd);
                _invested = false;
                Debug($"Sold BTC @{data.Bars[_btcusd].Close}$/Btc; Portfolio: {Portfolio.Cash}$, {Portfolio[_btcusd].Quantity}BTCs, Total Value: {Portfolio.TotalPortfolioValue}$, Total Fees: {Portfolio.TotalFees}$");
            }
        }
    }
}
