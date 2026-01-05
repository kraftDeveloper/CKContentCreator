using CKContentCreator.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WordPressPCL;
using WordPressPCL.Models;
using WordPressPCL.Utility;

namespace CKContentCreator.Pages
{
    public class ManagePostsModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public ManagePostsModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public List<Post> Posts { get; set; }
        public string BlogName { get; set; }

        public async Task OnGetAsync(string blogName)
        {
            BlogName = blogName;
            var blog = _configuration.GetSection("Blogs").Get<List<Blog>>().FirstOrDefault(b => b.Name == blogName);
            if (blog == null) return;

            var client = new WordPressClient($"{blog.Url}/wp-json/");
            client.Auth.UseBasicAuth(blog.Username, blog.Password);

            Posts = (await client.Posts.QueryAsync(new PostsQueryBuilder { PerPage = 20 })).ToList();
        }
    }
}