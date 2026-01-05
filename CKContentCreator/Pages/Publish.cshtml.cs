using CKContentCreator.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WordPressPCL;
using WordPressPCL.Models;

namespace CKContentCreator.Pages
{
    public class PublishModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public PublishModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public PostIdea Idea { get; set; }
        public List<Blog> Blogs { get; set; }

        [BindProperty]
        public string SelectedBlog { get; set; }

        public void OnGet(System.Guid id)
        {
            Idea = PostIdeaStorage.Ideas.Find(i => i.Id == id);
            Blogs = _configuration.GetSection("Blogs").Get<List<Blog>>();
        }

        public async Task<IActionResult> OnPostAsync(System.Guid id)
        {
            Idea = PostIdeaStorage.Ideas.Find(i => i.Id == id);
            var blog = Blogs.FirstOrDefault(b => b.Name == SelectedBlog);
            if (blog == null) return Page();

            var client = new WordPressClient($"{blog.Url}/wp-json/");
            client.Auth.UseBasicAuth(blog.Username, blog.Password);

            int? mediaId = null;
            if (Idea.Image != null && Idea.Image.Length > 0)
            {
                await using var stream = new MemoryStream(Idea.Image);
                var media = await client.Media.CreateAsync(stream, $"{Idea.Topic.Replace(" ", "-")}.png");
                mediaId = media.Id;
            }

            var post = new Post
            {
                Title = new Title(Idea.Topic),
                Content = new Content(Idea.Content),
                Status = Status.Publish,
                FeaturedMedia = mediaId
            };

            await client.Posts.CreateAsync(post);

            PostIdeaStorage.Ideas.Remove(Idea);

            return RedirectToPage("/PostIdeas");
        }
    }
}