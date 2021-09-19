using BlogLab.Core.BlogComment;
using BlogLab.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace BlogLab.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogCommentController : Controller
    {
        private readonly IBlogCommentRepository _blogCommentRepository;

        public BlogCommentController(IBlogCommentRepository blogCommentRepository)
        {
            _blogCommentRepository = blogCommentRepository;
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<BlogComment>> Create(BlogCommentCreate blogCommentCreate)
        {
            int applicationUserId = int.Parse(User.Claims.First(i => i.Type == JwtRegisteredClaimNames.NameId).Value);

            var createdBlogComment = await _blogCommentRepository.UpsertAsync(blogCommentCreate, applicationUserId);
            return Ok(createdBlogComment);
        }

        [HttpGet("{blogId}")]
        public async Task<ActionResult<BlogComment>> GetAll(int blogId)
        {
            var blogComments = await _blogCommentRepository.GetAllAsync(blogId);
            return Ok(blogComments);
        }

        [Authorize]
        [HttpDelete("{blogId}")]
        public async Task<ActionResult<BlogComment>> Delete(int blogCommentId)
        {
            int applicationUserId = int.Parse(User.Claims.First(i => i.Type == JwtRegisteredClaimNames.NameId).Value);

            var foundBlogComment = await _blogCommentRepository.GetAsync(blogCommentId);
            if (foundBlogComment == null)
                return BadRequest("Blog comment does not exists");
            else
            {
                if (foundBlogComment.ApplicationUserId != applicationUserId)
                    return BadRequest("You are not authorized to delete this comment");
                else
                {
                    int affectedrows = await _blogCommentRepository.DeleteAsync(blogCommentId);
                    return Ok(affectedrows);
                }
            }
        }
    }
}
