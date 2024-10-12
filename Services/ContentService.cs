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
            string errorMessage = string.Empty;
            var token = Configuration["GitHubToken"];

            if (string.IsNullOrEmpty(token))
            {
                errorMessage = "Failed to load content. No API token found.";
                return content;
            }

            // Add the token to the Authorization header
            Http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("token", token);

            var repoApiUrl = "https://api.github.com/repos/ACRonm/MyBlogPosts/contents/My%20Blog%20Posts";

            var response = await Http.GetFromJsonAsync<List<GitHubFile>>(repoApiUrl);

            if (response == null)
            {
                errorMessage = "Failed to load content. No files found.";
                return content;
            }

            foreach (var file in response)
            {
                var fileResponse = await Http.GetFromJsonAsync<GitHubFileContent>(file.Url);

                if (fileResponse != null && !string.IsNullOrEmpty(fileResponse.Content))
                {
                    var markdown = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(fileResponse.Content));

                    markdown = TruncateText(markdown, 50);

                    var htmlContent = Markdig.Markdown.ToHtml(markdown);

                    markdownContent.Add(htmlContent);

                }
                else
                {
                    errorMessage = "Failed to load content. File content is empty.";
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
        }

        private class GitHubFileContent
        {
            public string? Content { get; set; }
        }

    }
}

