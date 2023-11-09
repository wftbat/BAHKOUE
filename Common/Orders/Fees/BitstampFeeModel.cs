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
using QuantConnect.Util;
using QuantConnect.Securities;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Concurrent;

namespace QuantConnect.Orders.Fees
{
    /// <summary>
    /// Provides an implementation of <see cref="FeeModel"/> that models Bitstamp order fees
    /// </summary>
    public class BitstampFeeModel : FeeModel
    {
        private readonly TimeSpan RollingPeriod = TimeSpan.FromDays(30);
        private readonly ConcurrentDictionary<Security, ConcurrentQueue<(DateTime time,decimal totalVolume, decimal volume)>> _30daySaleVolumes = new ();


        //public Queue<(DateTime time, decimal volume)> ExecutedTrades { get; set; } = new();

        private int _tierIndex;

        public decimal MakerFee => _feeTiers[_tierIndex].MakerFee;
        public decimal TakerFee => _feeTiers[_tierIndex].TakerFee;


        /// <summary>
        /// Get the fee for this order in quote currency
        /// </summary>
        /// <param name="parameters">A <see cref="OrderFeeParameters"/> object
        /// containing the security and order</param>
        /// <returns>The cost of the order in quote currency</returns>
        public override OrderFee GetOrderFee(OrderFeeParameters parameters)
        {

            var order = parameters.Order;
            var security = parameters.Security;

            // Adjust schedule to account for volume in the last 30 days
            if (order.Status == OrderStatus.Submitted)
            {
                this.AdjustSchedule(parameters);
            }

            // Get order value in quote currency
            
            var unitPrice = order.Direction == OrderDirection.Buy ? security.AskPrice : security.BidPrice;
            unitPrice *= security.SymbolProperties.ContractMultiplier;

            var orderValue = unitPrice * order.AbsoluteQuantity;

            // Determine if the order is a maker or taker
            var isMaker = order.Type == OrderType.Limit && !order.IsMarketable;

            // Calculate the fee based on Bitstamp's fee schedule
            var feePercentage = isMaker ? MakerFee : TakerFee;
            
            var fee = orderValue * feePercentage;

            // Return the new OrderFee
            return new OrderFee(new CashAmount(fee, security.QuoteCurrency.Symbol));
        }

        private void AdjustSchedule(OrderFeeParameters parameters)
        {

            var targetTime = parameters.Order.Time - this.RollingPeriod ;

            var securityRollingVolumes = this._30daySaleVolumes.GetOrAdd(parameters.Security,
                security => new ConcurrentQueue<(DateTime time, decimal totalVolume, decimal volume)>());

            // Remove any volumes that are older than the rolling period

            while (securityRollingVolumes.TryPeek(out var oldestTrade) && oldestTrade.time < targetTime)
            {
                securityRollingVolumes.TryDequeue(out _);
            }

            var lastTotalVolume = securityRollingVolumes.Any() ? securityRollingVolumes.Last().totalVolume : 0m;
            var currentTotalVolume = parameters.Security.Holdings.TotalSaleVolume;

            if (currentTotalVolume > lastTotalVolume)
            {
                //todo: the time and volume is really just an approximation here, the volume should be split and timed between the latest executed trades
                securityRollingVolumes.Enqueue((parameters.Order.Time, currentTotalVolume, currentTotalVolume - lastTotalVolume));
            }

            var periodVolume = securityRollingVolumes.Sum(x => x.volume);

            // Iterate through the fee tiers from the lowest to find where the volume fits
            for (var tierIndex = 0; tierIndex < _feeTiers.Count; tierIndex++)
            {
                var tier = _feeTiers[tierIndex];
                if (periodVolume < tier.Volume)
                {
                    this._tierIndex = tierIndex - 1;
                    break;
                }

                if (tierIndex == _feeTiers.Count - 1)
                {
                    this._tierIndex = tierIndex;
                }
            }
            
        }

        // Define a list to hold volume tiers and corresponding fees, starting from the lowest

        private List<(decimal Volume, decimal MakerFee, decimal TakerFee)> _feeTiers = new List<(decimal Volume, decimal MakerFee, decimal TakerFee)>
        {
            (0m, 0.00m, 0.00m),
            (1000m, 0.30m, 0.40m),
            (10000m, 0.20m, 0.30m),
            (100000m, 0.10m, 0.20m),
            (500000m, 0.08m, 0.18m),
            (1500000m, 0.06m, 0.16m),
            (5000000m, 0.03m, 0.12m),
            (20000000m, 0.02m, 0.10m),
            (50000000m, 0.01m, 0.08m),
            (100000000m, 0.00m, 0.06m),
            (250000000m, 0.00m, 0.05m),
            (1000000000m, 0.00m, 0.03m)
        };

    }
}
