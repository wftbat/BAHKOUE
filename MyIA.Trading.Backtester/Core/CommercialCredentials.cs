using Aricie.DNN.UI.Attributes;
using System;
using System.Diagnostics;

namespace MyIA.Trading.Backtester
{
	[Serializable]
	public class CommercialCredentials
	{
		public bool Reload
		{
			[DebuggerNonUserCode]
			get;
			[DebuggerNonUserCode]
			set;
		}

		[ConditionalVisible("Reload", false, true)]
		public string ReloadAddress
		{
			[DebuggerNonUserCode]
			get;
			[DebuggerNonUserCode]
			set;
		}

		[DebuggerNonUserCode]
		public CommercialCredentials()
		{
		}
	}
}