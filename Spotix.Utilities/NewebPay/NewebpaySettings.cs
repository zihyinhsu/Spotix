using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spotix.Utilities.NewebPay
{
	public class NewebpaySettings
	{
		public static readonly string MerchantID = "MS154870975";
		public static readonly string HASHKEY = "xgSW8fKaKjj2UeMKo31mSDBRm7dOGeZb";
		public static readonly string HASHIV = "CG31aE46nTW77lLP";
		public static readonly string NotifyUrl = "https://3a19-114-34-72-78.ngrok-free.app/api/orders/notify";
		public static readonly string ReturnUrl = "https://spo-tix.vercel.app/events/area/orderResult";
		public static readonly string Version = "2.0";
		public static readonly string RespondType = "JSON";
	}
}
