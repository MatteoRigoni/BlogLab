using BlogLab.Core;
using BlogLab.Core.Blog;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogLab.Repository
{
    public class BlogRepository : IBlogRepository
    {
        private readonly IConfiguration _config;

        public BlogRepository(IConfiguration config)
        {
            _config = config;
        }

        public async Task<int> DeleteAsync(int blogId)
        {
            int affected = 0;
            using (var connection = new SqlConnection(_config.GetConnectionString("Default")))
            {
                await connection.OpenAsync();
                affected = await connection.ExecuteAsync("Blog_Delete",
                    new { BlogId = blogId },
                    commandType: CommandType.StoredProcedure);
            }
            return affected;
        }

        public async Task<PageResults<Blog>> GetAllAsync(BlogPaging blogPaging)
        {
            var blogs = new PageResults<Blog>();

            using (var connection = new SqlConnection(_config.GetConnectionString("Default")))
            {
                await connection.OpenAsync();

                using (var multi = await connection.QueryMultipleAsync("Blog_GetAll",
                    new
                    {
                        Offset = (blogPaging.Page - 1) * blogPaging.PageSize,
                        PageSize = blogPaging.PageSize
                    },
                    commandType: CommandType.StoredProcedure))
                {
                    blogs.Items = multi.Read<Blog>();
                    blogs.TotalCount = multi.ReadFirst<int>();
                }
            }

            return blogs;
        }

        public async Task<List<Blog>> GetAllByUserIdAsync(int applicationUserID)
        {
            IEnumerable<Blog> blogs = null;

            using (var connection = new SqlConnection(_config.GetConnectionString("Default")))
            {
                await connection.OpenAsync();
                blogs = await connection.QueryAsync<Blog>("Blog_GetByUserId",
                    new { ApplicationUserId = applicationUserID },
                    commandType: CommandType.StoredProcedure);
            }

            return blogs.ToList();
        }

        public async Task<List<Blog>> GetAllFamousAsync()
        {
            IEnumerable<Blog> blogs = null;

            using (var connection = new SqlConnection(_config.GetConnectionString("Default")))
            {
                await connection.OpenAsync();
                blogs = await connection.QueryAsync<Blog>("Blog_GetAllFamous",
                    new {  },
                    commandType: CommandType.StoredProcedure);
            }

            return blogs.ToList();
        }

        public async Task<Blog> GetAsync(int blogId)
        {
            Blog blog = null;

            using (var connection = new SqlConnection(_config.GetConnectionString("Default")))
            {
                await connection.OpenAsync();
                blog = await connection.QueryFirstOrDefaultAsync<Blog>("Blog_Get",
                    new { BlogId = blogId },
                    commandType: CommandType.StoredProcedure);
            }

            return blog;
        }

        public async Task<Blog> UpsertAsync(BlogCreate blogCreate, int applicationUserId)
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("BlogId", typeof(int));
            dataTable.Columns.Add("Title", typeof(string));
            dataTable.Columns.Add("Content", typeof(string));
            dataTable.Columns.Add("PhotoId", typeof(int));

            dataTable.Rows.Add(blogCreate.BlogId, blogCreate.Title, blogCreate.Content, blogCreate.PhotoId);

            int? newBlogId;

            using (var connection = new SqlConnection(_config.GetConnectionString("Default")))
            {
                await connection.OpenAsync();

                newBlogId = await connection.ExecuteScalarAsync<int?>(
                    "Blog_Upsert",
                    new { Blog = dataTable.AsTableValuedParameter("dbo.BlogType"), ApplicationUserId = applicationUserId },
                    commandType: CommandType.StoredProcedure
                    );
            }

            newBlogId = newBlogId ?? blogCreate.BlogId;

            Blog blog = await GetAsync(newBlogId.Value);

            return blog;
        }
    }
}
