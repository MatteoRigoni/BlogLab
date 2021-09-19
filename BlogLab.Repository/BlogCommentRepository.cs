using BlogLab.Core.BlogComment;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace BlogLab.Repository
{
    public class BlogCommentRepository : IBlogCommentRepository
    {
        private readonly IConfiguration _config;

        public BlogCommentRepository(IConfiguration config)
        {
            _config = config;
        }

        public async Task<int> DeleteAsync(int blogCommentId)
        {
            int affected = 0;
            using (var connection = new SqlConnection(_config.GetConnectionString("Default")))
            {
                await connection.OpenAsync();
                affected = await connection.ExecuteAsync("BlogComment_Delete",
                    new { BlogCommentId = blogCommentId },
                    commandType: CommandType.StoredProcedure);
            }
            return affected;
        }

        public async Task<List<BlogComment>> GetAllAsync(int blogId)
        {
            IEnumerable<BlogComment> blogComments = null;

            using (var connection = new SqlConnection(_config.GetConnectionString("Default")))
            {
                await connection.OpenAsync();
                blogComments = await connection.QueryAsync<BlogComment>("BlogComment_GetAll",
                    new { BlogId = blogId },
                    commandType: CommandType.StoredProcedure);
            }

            return blogComments.ToList();
        }

        public async Task<BlogComment> GetAsync(int blogCommentId)
        {
            BlogComment blogComment = null;

            using (var connection = new SqlConnection(_config.GetConnectionString("Default")))
            {
                await connection.OpenAsync();
                blogComment = await connection.QueryFirstOrDefaultAsync<BlogComment>("BlogComment_Get",
                    new { BlogCommentId = blogCommentId },
                    commandType: CommandType.StoredProcedure);
            }

            return blogComment;
        }

        public async Task<BlogComment> UpsertAsync(BlogCommentCreate blogCommentCreate, int applicationUserId)
        {
            int? newBlogCommentId = null;
            var dataTable = new DataTable();
            dataTable.Columns.Add("BlogCommentId", typeof(int));
            dataTable.Columns.Add("ParentBlogCommentId", typeof(int));
            dataTable.Columns.Add("BlogId", typeof(int));
            dataTable.Columns.Add("Content", typeof(string));

            dataTable.Rows.Add(
                blogCommentCreate.BlogCommentId,
                blogCommentCreate.ParentBlogCommentId,
                blogCommentCreate.BlogId,
                blogCommentCreate.Content);

            using (var connection = new SqlConnection(_config.GetConnectionString("Default")))
            {
                await connection.OpenAsync();
                newBlogCommentId = await connection.ExecuteScalarAsync<int>("BlogComment_Upsert",
                    new { BlogComment = dataTable.AsTableValuedParameter("dbo.BlogCommentType"), ApplicationUserId = applicationUserId },
                    commandType: CommandType.StoredProcedure);
            }

            newBlogCommentId = newBlogCommentId ?? blogCommentCreate.BlogCommentId;
            BlogComment blogComment = await GetAsync(newBlogCommentId.Value);
            return blogComment;
        }
    }
}
