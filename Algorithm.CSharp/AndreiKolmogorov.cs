using QuantConnect.Algorithm;
using QuantConnect.Data;
using QuantConnect.Indicators;
using QuantConnect.Data.Market;

namespace QuantConnect
{
    public class AndreiKolmogorov : QCAlgorithm
    {
        private MovingAverageConvergenceDivergence _macd;
        private readonly string _btcusd = "BTCUSD";
        private const decimal _tolerance = 0.0025m;
        private bool _invested;

        public override void Initialize()
        {
            SetStartDate(2023, 1, 1); // début backtest
            SetEndDate(2023, 12, 31); // fin backtest
            SetCash(10000); // capital

            AddCrypto(_btcusd, Resolution.Daily);
            _macd = MACD(_btcusd, 12, 26, 9, MovingAverageType.Exponential, Resolution.Daily, Field.Close);
        }

        public override void OnData(Slice data)
        {
            if (!_macd.IsReady) return;

            var holdings = Portfolio[_btcusd].Quantity;

            if (holdings <= 0 && _macd > _macd.Signal * (1 + _tolerance)) // condition d'achat
            {
                SetHoldings(_btcusd, 1.0);
                _invested = true;
            }
            else if (_invested && _macd < _macd.Signal) // condition de vente
            {
                Liquidate(_btcusd);
                _invested = false;
            }
        }
    }
}
