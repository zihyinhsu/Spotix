using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spotix.Utilities.NewebPay
{
	public class NewebpayResult
	{
		public string MerchantID { get; set; }
		public int Amt { get; set; }
		public string TradeNo { get; set; }
		public string MerchantOrderNo { get; set; }
		public string RespondType { get; set; }
		public string IP { get; set; }
		public string EscrowBank { get; set; }
		public string PaymentType { get; set; }
		public DateTime PayTime { get; set; }
		public string PayerAccount5Code { get; set; }
		public string PayBankCode { get; set; }
	}

	public class NewebpayNotifyResponse
	{
		public string Status { get; set; }
		public string Message { get; set; }
		public NewebpayResult Result { get; set; }
	}
}
