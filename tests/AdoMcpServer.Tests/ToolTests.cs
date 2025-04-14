using System.Diagnostics;
using System.Text;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol.Transport;

namespace AdoMcpServer.Tests;

public class EchoToolTests
{
    [Fact]
    public async Task EchoTool_ShouldReturnExpectedResponse()
    {
        var arguments = new List<string>();

        var clientTransport = new StdioClientTransport(new()
        {
            Name = "Demo Server",
            Command = "dotnet",
            Arguments = [
                "run",
                "--project",
                "/workspaces/ADO-MCP/src/AdoMcpServer/AdoMcpServer.csproj",
                "--no-build",
            ],
        });

        var mcpClient = await McpClientFactory.CreateAsync(clientTransport);
        var tools = await mcpClient.ListToolsAsync();
        Assert.Equal(3,tools.Count);

    }
}
