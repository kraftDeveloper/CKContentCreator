using System;

namespace CKContentCreator.Models
{
    public class PostIdea
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Topic { get; set; }
        public string Content { get; set; }
        public byte[] Image { get; set; } // Placeholder for generated image bytes
        public string ImagePrompt { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
    }
}