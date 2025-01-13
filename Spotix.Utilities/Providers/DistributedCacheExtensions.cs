using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spotix.Utilities.Providers
{
	public static class DistributedCacheExtensions
	{
		public static async Task SetObjectAsJsonAsync(this IDistributedCache cache, string key, object value, DistributedCacheEntryOptions options)
		{
			var jsonString = JsonConvert.SerializeObject(value);
			await cache.SetStringAsync(key, jsonString, options);
		}

		public static async Task<T> GetObjectFromJsonAsync<T>(this IDistributedCache cache, string key)
		{
			var jsonString = await cache.GetStringAsync(key);
			return jsonString == null ? default(T) : JsonConvert.DeserializeObject<T>(jsonString);
		}
	}
}
