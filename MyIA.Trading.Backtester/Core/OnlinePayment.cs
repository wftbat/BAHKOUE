using System;
using System.Diagnostics;

namespace MyIA.Trading.Backtester
{
	[Serializable]
	public class OnlinePayment : Payment
	{
		public string Reference
		{
			[DebuggerNonUserCode]
			get;
			[DebuggerNonUserCode]
			set;
		}

		public OnlinePayment()
		{
			this.Reference = "";
		}
	}
}