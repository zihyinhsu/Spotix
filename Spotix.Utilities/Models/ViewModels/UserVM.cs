using Spotix.Utilities.Models.EFModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spotix.Utilities.Models.ViewModels
{
    public class UserVM
    {
        [Display(Name = "Line ID")]
        [StringLength(100)]
        public string LineId { get; set; }

        [Display(Name = "性別")]
        [Required(ErrorMessage = "{0} 為必填")]
        public bool Gender { get; set; }

        [Display(Name = "生日")]
        [Required(ErrorMessage = "{0} 為必填")]
        public DateTime Birthday { get; set; }

        [Display(Name = "地址")]
        [StringLength(100)]
        public string Address { get; set; }

        [Display(Name = "大頭貼")]
        public string AvatarUrl { get; set; }

        [Display(Name = "訂單列表")]
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

        [Display(Name = "身分列表")]
        [Required(ErrorMessage = "{0} 為必填")]
        public List<string> Roles { get; set; }
    }
}
