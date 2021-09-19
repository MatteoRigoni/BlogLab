using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BlogLab.Core.BlogComment
{
    public class BlogCommentCreate
    {
        public int BlogCommentId { get; set; }
        public int? ParentBlogCommentId { get; set; }
        public int BlogId { get; set; }
        [Required]
        [MinLength(5, ErrorMessage = "Must be at least 5-100 characters")]
        [MaxLength(100, ErrorMessage = "Must be at least 5-100 characters")]
        public string Content { get; set; }
    }
}
