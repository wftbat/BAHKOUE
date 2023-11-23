using System;
using System.Diagnostics;

namespace MyIA.Trading.Backtester
{
	[Serializable]
	public class WalletPayment : Payment
	{
		public MyIA.Trading.Backtester.Balance Balance
		{
			[DebuggerNonUserCode]
			get;
			[DebuggerNonUserCode]
			set;
		}

		[DebuggerNonUserCode]
		public WalletPayment()
		{
		}
	}
}
