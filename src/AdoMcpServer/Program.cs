using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Text;

const string ACCESS_TOKEN= "ADO_MCP_PAT";
const string ADO_ORG= "ADO_MCP_ORG";

var builder = Host.CreateEmptyApplicationBuilder(settings: null);

builder.Logging.AddConsole(consoleLogOptions =>
{
    consoleLogOptions.LogToStandardErrorThreshold = LogLevel.Trace;
});

builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly();

builder.Services.AddSingleton(static _ =>
{
    var pat = Environment.GetEnvironmentVariable(ACCESS_TOKEN);
    var token = Convert.ToBase64String(Encoding.ASCII.GetBytes($":{pat}"));
    var orgName = Environment.GetEnvironmentVariable(ADO_ORG);
    if (string.IsNullOrEmpty(orgName))
    {
        throw new InvalidOperationException($"Environment variable {ADO_ORG} is not set.");
    }
    var client = new HttpClient() { BaseAddress = new Uri($"https://dev.azure.com/{orgName}/") };
    client.DefaultRequestHeaders.Accept.Clear();
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", token);
    return client;
});

builder.Services.AddSingleton(static _ =>
{
    var memory = new Dictionary<string, string>
    {
        ["org_name"] = Environment.GetEnvironmentVariable(ADO_ORG) ?? string.Empty
    };
    return memory;
});

await builder.Build().RunAsync();