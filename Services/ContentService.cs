using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;
using System.Reflection.Metadata;
using System.Text.Json;
using HtmlAgilityPack;
using Markdig;


namespace MyPortfolio.Services
{
	public class ContentService
	{

		private readonly IConfiguration Configuration;
		private readonly HttpClient Http = new HttpClient();
		private List<GitHubFile>? GitHubFiles = new List<GitHubFile>();
		private string errorMessage = string.Empty;


		public ContentService(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public async Task<List<GitHubFile>> LoadContentAsync()
		{
			string token = "";

			if (GitHubFiles != null && GitHubFiles.Count > 0)
			{
				GitHubFiles.Clear();
            }

			// Add the token to the Authorization header
			Http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("token", token);

			var apiUrl = "https://myportfolioapi-arg0ejd5hbc9bxa7.australiaeast-01.azurewebsites.net/blogpost";
			var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
			var response = await Http.SendAsync(request);
			response.EnsureSuccessStatusCode();


			GitHubFiles = await response.Content.ReadFromJsonAsync<List<GitHubFile>>();

			if (GitHubFiles == null)
				return new List<GitHubFile>();

			foreach (var file in GitHubFiles)
			{
				if (!string.IsNullOrEmpty(file.Name))
				{
					file.Name = Path.GetFileNameWithoutExtension(file.Name);
				}

				file.Content = file.Content != null ? ParseMarkdownToHtml(file.Content) : string.Empty;

				var doc = new HtmlDocument();
                doc.LoadHtml(file.Content);

                file.ImageUrl = ParseTeaserImage(doc);

				ExtractTags(file, doc);

            }
            return GitHubFiles;
        }

        public string ParseMarkdownToHtml(string content)
        {
            content = content != null ? Markdown.ToHtml(content) : string.Empty;


            return content;
        }

        public string ParseTeaserImage(HtmlDocument content)
        {
            var imgNode = content.DocumentNode.SelectSingleNode("//img");
            string imageUrl = imgNode != null ? imgNode.GetAttributeValue("src", "defaultImage.jpg") : "defaultImage.jpg";

            return imageUrl;
        }


        // TODO: Ensure that tags are correctly removed from the document and returned as a list of strings
        public void ExtractTags(GitHubFile file, HtmlDocument content)
        {
            var tags = new List<string>();
            var tagsNode = content.DocumentNode.SelectSingleNode("//p[text()='tags:']/following-sibling::ul");
            var tagsParagraph = content.DocumentNode.SelectSingleNode("//p[text()='tags:']");

			if (tagsNode != null)
			{
				foreach (var tag in tagsNode.SelectNodes("li"))
				{
					tags.Add(tag.InnerText);
					tagsNode.Remove();
				}
			}
			if (tagsParagraph != null)
			{
				tagsParagraph.Remove();
			}

			file.Content = content.DocumentNode.InnerHtml;

			file.Tags = tags;
		}
		public class GitHubFile
		{
			public string? Name { get; set; }
			public string? Title { get; set; }
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

