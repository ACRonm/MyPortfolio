using System;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;


namespace MyPortfolio.Services
{
    public class ContentService {

        private readonly IConfiguration Configuration;
        private readonly HttpClient Http = new HttpClient();
        private List<GitHubFile>? GitHubFiles = new List<GitHubFile>();


        public ContentService(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        private List<string> markdownContent = new List<string>();

        public async Task<List<GitHubFile>> LoadContentAsync()
        {
            var token = Configuration["GitHubToken"];

            if (GitHubFiles != null && GitHubFiles.Count > 0)
            {
                GitHubFiles.Clear();
            }
            
            // Add the token to the Authorization header
            Http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("token", token);

            var repoApiUrl = "https://api.github.com/repos/ACRonm/MyBlogPosts/contents/My%20Blog%20Posts";
            GitHubFiles = await Http.GetFromJsonAsync<List<GitHubFile>>(repoApiUrl);

            if (GitHubFiles == null)
                return new List<GitHubFile>();

            foreach (var file in GitHubFiles)
            {

                var fileResponse = await Http.GetFromJsonAsync<GitHubFile>(file.Url);

                if (fileResponse?.Content != null)
                {
                    file.Content = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(fileResponse.Content));
                }

                var commitApiUrl = $"https://api.github.com/repos/ACRonm/MyBlogPosts/commits?path={file.Path}&page=1&per_page=1";
                var commitResponse = await Http.GetFromJsonAsync<JsonElement[]>(commitApiUrl);

                if (commitResponse != null && commitResponse.Length > 0)
                {
                    file.CommitDate = commitResponse[0].GetProperty("commit").GetProperty("committer").GetProperty("date").GetString();
                    file.Committer = commitResponse[0].GetProperty("commit").GetProperty("committer").GetProperty("name").GetString();
                }
            }
            return GitHubFiles;
        }

        public class GitHubFile
        {
            public string? Name { get; set; }
            public string? Url { get; set; }
            public string? Content { get; set; }
            public string? CommitDate { get; set; }
            public string? Committer { get; set; }
            public string? Path { get; set; }
            public string? Encoding { get; set; }
            public string? ImageUrl { get; set; }
            public List<string>? Tags { get; set; }
        }
    }
}

