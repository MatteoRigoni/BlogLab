using BlogLab.Core.Blog;
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
    public class BlogController : Controller
    {
        private readonly IBlogRepository _blogRepository;
        private readonly IPhotoRepository _photoRepository;

        public BlogController(IPhotoRepository photoRepository, IBlogRepository blogRepository)
        {
            _photoRepository = photoRepository;
            _blogRepository = blogRepository;
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Blog>> Create(BlogCreate blogCreate)
        {
            int applicationUserId = int.Parse(User.Claims.First(i => i.Type == JwtRegisteredClaimNames.NameId).Value);

            if (blogCreate.PhotoId.HasValue)
            {
                var photo = await _photoRepository.GetAsync(blogCreate.PhotoId.Value);
                if (photo.ApplicationUserId != applicationUserId)
                    return BadRequest("You did not upload the photo");
            }

            var blog = await _blogRepository.UpsertAsync(blogCreate, applicationUserId);
            return Ok(blog);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Blog>>> GetAll([FromQuery] BlogPaging blogPaging)
        {
            var blogs = await _blogRepository.GetAllAsync(blogPaging);
            return Ok(blogs);
        }

        [HttpGet("{blogId}")]
        public async Task<ActionResult<Blog>> Get(int blogId)
        {
            var blog = await _blogRepository.GetAsync(blogId);
            return Ok(blog);
        }

        [HttpGet("user/{applicationUserId}")]
        public async Task<ActionResult<IEnumerable<Blog>>> GetByApplicationUserId(int applicationUserId)
        {
            var blogs = await _blogRepository.GetAllByUserIdAsync(applicationUserId);
            return Ok(blogs);
        }

        [HttpGet("famous")]
        public async Task<ActionResult<IEnumerable<Blog>>> GetAllFamous()
        {
            var blogs = await _blogRepository.GetAllFamousAsync();
            return Ok(blogs);
        }

        [Authorize]
        [HttpDelete("{blogId}")]
        public async Task<ActionResult<int>> Delete(int blogId)
        {
            int applicationUserId = int.Parse(User.Claims.First(i => i.Type == JwtRegisteredClaimNames.NameId).Value);

            var foundBlog = await _blogRepository.GetAsync(blogId);
            if (foundBlog == null)
                return BadRequest("Blog does not exists");
            else
            {
                if (foundBlog.ApplicationUserId != applicationUserId)
                    return BadRequest("You are not authorized to delete this blog");
                else
                {
                    int affectedRows = await _blogRepository.DeleteAsync(blogId);
                    return Ok(affectedRows);
                }
            }
        }
    }
}
