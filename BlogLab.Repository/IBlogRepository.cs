using BlogLab.Core;
using BlogLab.Core.Blog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BlogLab.Repository
{
    public interface IBlogRepository
    {
        Task<Blog> UpsertAsync(BlogCreate blogCreate, int applicationUserId);
        Task<PageResults<Blog>> GetAllAsync(BlogPaging blogPaging);
        Task<Blog> GetAsync(int blogId);
        Task<List<Blog>> GetAllByUserIdAsync(int applicationUserID);
        Task<List<Blog>> GetAllFamousAsync();
        Task<int> DeleteAsync(int blogId);
    }
}
