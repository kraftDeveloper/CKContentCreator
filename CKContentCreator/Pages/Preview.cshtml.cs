using Microsoft.AspNetCore.Mvc.RazorPages;
using CKContentCreator.Models;
using System;

namespace CKContentCreator.Pages
{
    public class PreviewModel : PageModel
    {
        public PostIdea Idea { get; set; }

        public void OnGet(Guid id)
        {
            Idea = PostIdeaStorage.Ideas.Find(i => i.Id == id);
        }
    }
}