//using Jayrock.Json;
//using Jayrock.Json.Conversion;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace MyIA.Trading.Backtester
{
	[Serializable]
	public class SimulationData
	{
		private string _JsonWallet;

		private string _JsonTicker;

		private string _JsonMarketDepth;

		
		public string JsonMarketDepth
		{
			get
			{
				return this._JsonMarketDepth;
			}
			set
			{
				this._JsonMarketDepth = value;
			}
		}

		public string JsonTicker
		{
			get
			{
				return this._JsonTicker;
			}
			set
			{
				this._JsonTicker = value;
			}
		}

		public string JsonWallet
		{
			get
			{
				return this._JsonWallet;
			}
			set
			{
				this._JsonWallet = value;
			}
		}

		[Browsable(false)]
		[XmlIgnore()]
		[JsonIgnore()]
        public MarketInfo Market
		{
			get
			{
                //todo: migrate the following to  Newtonsoft json.net
                //MarketDepth objMarketDepth;
                //TickerInfo objTickerInfo;
                //Jayrock.Json.Conversion.ImportContext impContext = new Jayrock.Json.Conversion.ImportContext();
                //impContext.Register(impContext.FindImporter(typeof(Ticker)));
                //using (StringReader reader = new StringReader(this._JsonTicker))
                //{
                //    objTickerInfo = (TickerInfo)impContext.Import(typeof(TickerInfo), JsonText.CreateReader(reader));
                //}
                //impContext = new Jayrock.Json.Conversion.ImportContext();
                //using (StringReader reader = new StringReader(this._JsonMarketDepth))
                //{
                //    objMarketDepth = (MarketDepth)impContext.Import(typeof(MarketDepth), JsonText.CreateReader(reader));
                //}
                //return new MarketInfo(objTickerInfo.ticker, objMarketDepth);
                return new MarketInfo();
			}
		}

		[Browsable(false)]
		[XmlIgnore()]
		[JsonIgnore()]
        public MyIA.Trading.Backtester.Wallet Wallet
		{
			get
			{
                //todo: migrate the following to  Newtonsoft json.net
				MyIA.Trading.Backtester.Wallet toReturn = new Wallet();
                //Jayrock.Json.Conversion.ImportContext impContext = new Jayrock.Json.Conversion.ImportContext();
                //impContext.Register(impContext.FindImporter(typeof(List<Order>)));
                //using (StringReader reader = new StringReader(this._JsonWallet))
                //{
                //    toReturn = (MyIA.Trading.Backtester.Wallet)impContext.Import(typeof(MyIA.Trading.Backtester.Wallet), JsonText.CreateReader(reader));
                //}
				return toReturn;
			}
		}

		[DebuggerNonUserCode]
		public SimulationData()
		{
		}
	}
}
