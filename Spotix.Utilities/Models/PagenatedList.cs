using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spotix.Utilities.Models
{
	public class PaginatedList<T> : List<T>
	{
		public int PageIndex { get; private set; }
		public int TotalPages { get; private set; }

		public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
		{
			PageIndex = pageIndex;
			TotalPages = (int)Math.Ceiling(count / (double)pageSize);
			//Math.Ceiling: 這是 C# 中的數學函數，用於將浮點數向上取整。
			//例如，Math.Ceiling(2.3) 會返回 3，Math.Ceiling(2.0) 會返回 2。這樣可以確保即使有部分頁面也會計算在內。
			this.AddRange(items);
		}

		public bool HasPreviousPage
		//HasPreviousPage:判斷上一頁面是否存在，用來啟動跟關閉上一頁按鈕。
		{
			get
			{
				return (PageIndex > 1);
			}
		}

		public bool HasNextPage
		//HasNextPage:判斷下一頁面，用來啟動跟關閉下一頁按鈕。
		{
			get
			{
				return (PageIndex < TotalPages);
			}
		}

		//CreateAsync:主要讓我們把資料以PaginatedList的方式傳給View。
		public static async Task<PaginatedList<T>> CreateAsyncP(IQueryable<T> source, int pageIndex, int pageSize)
		{
			var count = await source.CountAsync();
			var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
			return new PaginatedList<T>(items, count, pageIndex, pageSize);
		}

	}
}
