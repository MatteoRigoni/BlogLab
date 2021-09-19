using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BlogLab.Core.Blog
{
    public class BlogCreate
    {
        public int BlogId { get; set; }
        [Required]
        [MinLength(5, ErrorMessage = "Must be at least 5-20 characters")]
        [MaxLength(20, ErrorMessage = "Must be at least 5-20 characters")]
        public string Title { get; set; }
        public string Content { get; set; }
        public int? PhotoId { get; set; }
    }
}
