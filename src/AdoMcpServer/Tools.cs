using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AdoMcpServer
{
    [McpServerToolType]
    public static class Tools
    {
        public const string API_VERSION = "api-version=7.1";

        [
        McpServerTool(Title = "Get ADO project list", ReadOnly = true, Idempotent = true),
        Description("Get the list of projects for the current organization.")
        ]
        public static async Task<string> GetProjects(HttpClient client)
        {
            var projects = await client.GetFromJsonAsync<JsonElement>($"_apis/projects?{API_VERSION}");
            var names = projects.GetProperty("value")
                .EnumerateArray()
                .Select(p => p.GetProperty("name").GetString())
                .ToList();
            return string.Join("\n--\n", names);
        }

        [
            McpServerTool(Title = "Sets the current project", ReadOnly = false, Idempotent = false, Destructive = false),
            Description("Sets the name of the target project")
        ]
        public static string SetProject(Dictionary<string, string> memory, string project)
        {
            if (string.IsNullOrEmpty(project))
            {
                return "Project name cannot be empty.";
            }
            memory["project"] = project;
            return $"Project set to {project}.";
        }

        [
        McpServerTool(Title = "Search the wikis", ReadOnly = true, Idempotent = true),
        Description("Search the set of project wikis for something.")
        ]
        public static async Task<string> SearchWikis(HttpClient client, Dictionary<string, string> memory, string searchText)
        {
            memory.TryGetValue("project", out var projectName);
            if (string.IsNullOrEmpty(projectName))
            {
                return "Project name is not set. Use the SetProject tool to set it.";
            }

            var orgName = memory["org_name"];

            var response = await client.PostAsJsonAsync(
                $"https://almsearch.dev.azure.com/{orgName}/{projectName}/_apis/search/wikisearchresults?{API_VERSION}",
                 new Dictionary<string, object>
                 {
                    {"searchText", searchText},
                    {"$skip", 0},
                    {"$top", 10},
                    {"includeFacets", true}
                 });

            if (!response.IsSuccessStatusCode) return $"Error: {response.StatusCode}";
            var json = await response.Content.ReadFromJsonAsync<JsonElement>();
            var count = json.GetProperty("count").GetInt32();
            var results = json.GetProperty("results")
                .EnumerateArray()
                .ToList();

            return json.ToString();
        }
    }
}