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
        public string Id { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public bool Gender { get; set; }

        [DataType(DataType.Date)]
        public DateTime Birthday { get; set; }

        public string Address { get; set; }

        [DataType(DataType.Url)]
        public string AvatarUrl { get; set; }
    }
}
