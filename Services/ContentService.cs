using System;
using System.Net.Http.Json;


namespace MyPortfolio.Services
{
    public class ContentService {
        private List<string> content = new List<string>();
        private readonly IConfiguration Configuration;
        private readonly HttpClient Http = new HttpClient();

        public ContentService(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        private List<string> markdownContent = new List<string>();

        public async Task<List<string>> LoadContentAsync()
        {
            var token = Configuration["GitHubToken"];

            if (string.IsNullOrEmpty(token))
            {
                return content;
            }

            // If The content is already loaded, clear it to stop duplicates
            if (markdownContent.Count > 0)
                markdownContent.Clear();
            
            // Add the token to the Authorization header
            Http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("token", token);

            var repoApiUrl = "https://api.github.com/repos/ACRonm/MyBlogPosts/contents/My%20Blog%20Posts";

            var response = await Http.GetFromJsonAsync<List<GitHubFile>>(repoApiUrl);

            if (response == null)
            {
                return content;
            }
            foreach (var file in response)
            {
                var fileResponse = await Http.GetFromJsonAsync<GitHubFile>(file.Url);

                if (!string.IsNullOrEmpty(fileResponse?.Content))
                {

                    var markdown = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(fileResponse.Content));
                    markdownContent.Add(markdown);
                }
            }
            return markdownContent;
        }


        private string TruncateText(string text, int wordLimit)
        {
            if (string.IsNullOrEmpty(text)) return text;

            var words = text.Split(' ');
            if (words.Length <= wordLimit) return text;

            return string.Join(' ', words.Take(wordLimit)) + " ...";
        }

        private class GitHubFile
        {
            public string? Name { get; set; }
            public string? Url { get; set; }
            public string? Content { get; set; }
        }

    }
}

