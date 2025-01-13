using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Spotix.Utilities.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Spotix.Utilities.NewebPay
{
	public static class Crypto
	{
		// 字串組合
		public static string GenDataChain(EncryptOrderDto order)
		{
			return $"MerchantID={NewebpaySettings.MerchantID}&TimeStamp={order.TimeStamp}&Version={NewebpaySettings.Version}&RespondType={NewebpaySettings.RespondType}&MerchantOrderNo={order.MerchantOrderNo}&Amt={order.Amt}&Total={order.Total}&NotifyURL={Uri.EscapeDataString(NewebpaySettings.NotifyUrl)}&ReturnURL={Uri.EscapeDataString(NewebpaySettings.ReturnUrl)}&ItemDesc={Uri.EscapeDataString(order.ItemDesc)}&OrderNumber={Uri.EscapeDataString(order.OrderNumber)}&TicketIds={Uri.EscapeDataString(order.TicketIds)}&Email={Uri.EscapeDataString(order.Email ?? string.Empty)}&UserId={Uri.EscapeDataString(order.UserId)}&CreatedTime={Uri.EscapeDataString(order.CreatedTime.ToString("o"))}";
		}

		// AES 加密
		public static string AESEncrypt(EncryptOrderDto encryptOrder)
		{
			var key = Encoding.UTF8.GetBytes(NewebpaySettings.HASHKEY);
			var iv = Encoding.UTF8.GetBytes(NewebpaySettings.HASHIV);
			var data = Encoding.UTF8.GetBytes(GenDataChain(encryptOrder));

			using (var aes = Aes.Create())
			{
				aes.Key = key;
				aes.IV = iv;
				aes.Mode = CipherMode.CBC;
				aes.Padding = PaddingMode.PKCS7;

				using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
				{
					var encrypted = encryptor.TransformFinalBlock(data, 0, data.Length);
					return BitConverter.ToString(encrypted).Replace("-", "").ToLower();
				}
			}
		}

		// AES 解密
		public static NewebpayNotifyResponse AESDecrypt(string tradeInfo)
		{
			var key = Encoding.UTF8.GetBytes(NewebpaySettings.HASHKEY);
			var iv = Encoding.UTF8.GetBytes(NewebpaySettings.HASHIV);
			var encryptedHexStr = StringToByteArray(tradeInfo);

			using (var aes = Aes.Create())
			{
				aes.Key = key;
				aes.IV = iv;
				aes.Mode = CipherMode.CBC;
				aes.Padding = PaddingMode.PKCS7;

				using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
				{
					var decrypted = decryptor.TransformFinalBlock(encryptedHexStr, 0, encryptedHexStr.Length);
					var decryptedText = Encoding.UTF8.GetString(decrypted).TrimEnd('\0');

					var result =  JsonConvert.DeserializeObject<NewebpayNotifyResponse>(decryptedText);

					return result;


				}
			}
		}

		private static bool IsValidJson(string strInput)
		{
			strInput = strInput.Trim();
			if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || // For object
				(strInput.StartsWith("[") && strInput.EndsWith("]")))   // For array
			{
				try
				{
					var obj = JToken.Parse(strInput);
					return true;
				}
				catch (JsonReaderException)
				{
					return false;
				}
			}
			else
			{
				return false;
			}
		}

		// SHA256 加密
		public static string SHA256Encrypt(string aesEncrypt)
		{
			var plainText = $"HashKey={NewebpaySettings.HASHKEY}&{aesEncrypt}&HashIV={NewebpaySettings.HASHIV}";
			using (var sha256 = SHA256.Create())
			{
				var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(plainText));
				return BitConverter.ToString(hash).Replace("-", "").ToUpper();
			}
		}

		private static byte[] StringToByteArray(string hex)
		{
			int NumberChars = hex.Length;
			byte[] bytes = new byte[NumberChars / 2];
			for (int i = 0; i < NumberChars; i += 2)
				bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
			return bytes;
		}
	}

}


