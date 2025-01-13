using Spotix.Utilities.Models.EFModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spotix.Utilities.Models.DTOs
{
	public class NewebpayNotifyDto 
	{
		public string Status { get; set; }
		public string MerchantID { get; set; }
		public string Version { get; set; }
		public string TradeInfo { get; set; }
		public string TradeSha { get; set; }
	}
}
