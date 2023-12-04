using System;

namespace MyIA.Trading.Backtester
{
	public enum TradingAPIUrls
	{
		Ticker,
		MarketDepth,
		RecentTrades,
		GetBalance,
		BuyBTC,
		SellBTC,
		GetOrders,
		CancelOrder,
		SendBTC,
		GetDepositAddress,
        GetMarkets
	}
}