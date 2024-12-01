using System.Net.Http.Json;
using HtmlAgilityPack;
using Markdig;

namespace MyPortfolio.Services
{
	public class ContentService
	{

		private readonly HttpClient Http = new HttpClient();
		private List<GitHubFile>? GitHubFiles = new List<GitHubFile>();
		public readonly string baseApiUrl;

		public ContentService(IConfiguration configuration)
		{
			baseApiUrl = configuration["ApiSettings:BaseApiUrl"] ?? "";
		}

		public async Task<List<GitHubFile>> LoadContentAsync()
		{
			if (GitHubFiles != null && GitHubFiles.Count > 0)
			{
				GitHubFiles.Clear();
			}

			GitHubFiles = await Http.GetFromJsonAsync<List<GitHubFile>>($"{baseApiUrl}/blogpost");

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

