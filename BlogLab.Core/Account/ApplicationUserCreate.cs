using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BlogLab.Core.Account
{
    public class ApplicationUserCreate : ApplicationUserLogin
    {
        [Required]
        [MinLength(5, ErrorMessage = "Must be at least 5-20 characters")]
        [MaxLength(20, ErrorMessage = "Must be at least 5-20 characters")]
        public string Fullname { get; set; }
        [Required]
        [MinLength(5, ErrorMessage = "Must be at least 5-20 characters")]
        [MaxLength(50, ErrorMessage = "Must be at least 5-50 characters")]
        [EmailAddress(ErrorMessage = "Invalid mail format")]
        public string Email { get; set; }
    }
}
