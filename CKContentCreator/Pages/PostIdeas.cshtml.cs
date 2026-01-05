using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CKContentCreator.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace CKContentCreator.Pages
{
    public class PostIdeasModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _configuration;

        public PostIdeasModel(IHttpClientFactory clientFactory, IConfiguration configuration)
        {
            _clientFactory = clientFactory;
            _configuration = configuration;
        }

        public List<PostIdea> PostIdeas { get; set; } = new List<PostIdea>();

        [BindProperty]
        public string Topic { get; set; }

        public void OnGet()
        {
            PostIdeas = PostIdeaStorage.Ideas;
        }

        public async Task<IActionResult> OnPostGenerateAsync()
        {
            if (string.IsNullOrEmpty(Topic)) return Page();

            var ollamaUrl = _configuration["OllamaUrl"];
            var client = _clientFactory.CreateClient();

            // Generate content (adjust model as per your Ollama setup, e.g., 'llama2')
            var contentRequest = new { model = "llama2", prompt = $"Write a detailed blog post about '{Topic}' in HTML format." };
            var contentResponse = await client.PostAsJsonAsync($"{ollamaUrl}/api/generate", contentRequest);
            contentResponse.EnsureSuccessStatusCode();
            var contentJson = await contentResponse.Content.ReadAsStringAsync();
            var content = JsonDocument.Parse(contentJson).RootElement.GetProperty("response").GetString();

            // Generate image prompt
            var imagePromptRequest = new { model = "llama2", prompt = $"Create a descriptive prompt for generating an image that represents '{Topic}'." };
            var imagePromptResponse = await client.PostAsJsonAsync($"{ollamaUrl}/api/generate", imagePromptRequest);
            imagePromptResponse.EnsureSuccessStatusCode();
            var imagePromptJson = await imagePromptResponse.Content.ReadAsStringAsync();
            var imagePrompt = JsonDocument.Parse(imagePromptJson).RootElement.GetProperty("response").GetString();

            // Placeholder for image generation (Ollama doesn't generate images; integrate e.g., SD API here)
            byte[] imageBytes = Array.Empty<byte>(); // Replace with actual API call to gen image from prompt

            var idea = new PostIdea { Topic = Topic, Content = content, ImagePrompt = imagePrompt, Image = imageBytes };
            PostIdeaStorage.Ideas.Add(idea);

            return RedirectToPage();
        }

        public IActionResult OnPostDelete(Guid id)
        {
            PostIdeaStorage.Ideas.RemoveAll(i => i.Id == id);
            return RedirectToPage();
        }
    }

    public static class PostIdeaStorage
    {
        public static List<PostIdea> Ideas { get; } = new List<PostIdea>();
    }
}