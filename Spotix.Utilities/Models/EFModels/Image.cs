using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spotix.Utilities.Models.EFModels
{
	public partial class Image
	{
		[Key]
		public string Id { get; set; }
		public string ImageUrl { get; set; }
	}
}

