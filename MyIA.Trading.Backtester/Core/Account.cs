using System;


namespace MyIA.Trading.Backtester
{
	[Serializable]
	public class Account : ResponseObject
	{
	    public string AccountNumber { get; set; }

	    public Balance Balance { get; set; }

	    public Account()
		{
		}

		public Account(ResponseObject objresponseObject) : base(objresponseObject)
		{
		}
	}
}
