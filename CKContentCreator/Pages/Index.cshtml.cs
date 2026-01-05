using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using CKContentCreator.Models;
using System.Collections.Generic;

namespace CKContentCreator.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public IndexModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public List<Blog> Blogs { get; set; }

        public void OnGet()
        {
            Blogs = _configuration.GetSection("Blogs").Get<List<Blog>>();
        }

        public IActionResult OnPostSwitchTheme()
        {
            var currentTheme = HttpContext.Session.GetString("Theme") ?? "light";
            var newTheme = currentTheme == "light" ? "dark" : "light";
            HttpContext.Session.SetString("Theme", newTheme);
            return RedirectToPage();
        }
    }
}