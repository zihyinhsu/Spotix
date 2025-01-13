using Spotix.Utilities.Models.EFModels;
using Spotix.Utilities.NewebPay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spotix.Utilities.Models.DTOs
{
	public class OrderDto
	{
		public int Id { get; set; }

		public DateTime CreatedTime { get; set; }

		public int Total { get; set; }

		public string UserId { get; set; }

		public string OrderNumber { get; set; }

		public List<TicketDto> Tickets { get; set; }
	}

	public class EncryptOrderDto : OrderDto
	{
		public long TimeStamp { get; set; }
		public int Amt { get; set; }
		public string ItemDesc { get; set; }
		public string Email { get; set; }
		public string TradeInfo { get; set; }
		public string TradeSha { get; set; }
		public string MerchantID { get; set; }
		public long MerchantOrderNo { get; set; }
		public string Version { get; set; }
		public string NotifyUrl { get; set; }
		public string ReturnUrl { get; set; }
		public string TicketIds { get; set; }

	}
}
