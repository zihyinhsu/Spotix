using Spotix.Utilities.Models.DTOs.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spotix.Utilities.Models.DTOs.Messages
{
	public class TextMessageDto : BaseMessageDto
	{
		public TextMessageDto()
		{
			Type = MessageTypeEnum.Text.ToString();
		}


		[Required]
		public string Text { get; set; }

		public new string Type { get; } // 設置為只讀屬性

	}
}
