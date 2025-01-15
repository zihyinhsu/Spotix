using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spotix.Utilities.Models.ViewModels
{
    public class EditProfileVM
    {
        [Display(Name = "姓名")]
        [Required(ErrorMessage = "姓名為必填")]
        public string UserName { get; set; }

        [Display(Name = "性別")]
        [Required(ErrorMessage = "請選擇您的性別")]
        public bool Gender { get; set; }

        [Display(Name = "生日")]
        [Required(ErrorMessage = "生日為必填")]
        public DateTime Birthday { get; set; }

        [Display(Name = "居住地址")]
        public string? Address { get; set; } = null;

        [Display(Name = "手機號碼")]
        [Required(ErrorMessage = "手機號碼為必填")]
        public string PhoneNumber { get; set; }
    }
}
