using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spotix.Utilities.Models.ViewModels
{
    public class PlaceVM
    {
        [Display(Name = "場地名稱")]
        [Required(ErrorMessage = "{0} 為必填")]
        [StringLength(50)]
        public string Name { get; set; }

        [Display(Name = "顯示順序")]
        [Required(ErrorMessage = "{0} 為必填")]
        public int DisplayOrder { get; set; }

        [Display(Name = "是否可使用")]
        [Required(ErrorMessage = "{0} 為必填")]
        public bool Enabled { get; set; }

    }
}
