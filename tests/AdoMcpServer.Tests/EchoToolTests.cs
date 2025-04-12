using System.Diagnostics;
using System.Text;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol.Transport;

namespace AdoMcpServer.Tests;

public class EchoToolTests : IDisposable
{
    private readonly Process _serverProcess;

    public EchoToolTests()
    {
        // Start the server process
        _serverProcess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = "run --project ../../src/AdoMcpServer.Console/AdoMcpServer.Console.csproj",
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            }
        };

        _serverProcess.Start();

        // Give the server a moment to start up
        Thread.Sleep(1000);
    }

    public void Dispose()
    {
        // Clean up the server process when done
        if (!_serverProcess.HasExited)
        {
            _serverProcess.Kill();
        }
        _serverProcess.Dispose();
    }

    [Fact]
    public async Task EchoTool_ShouldReturnExpectedResponse()
    {
        var command = "";
        var arguments = new List<string>();

        var clientTransport = new StdioClientTransport(new()
        {
            Name = "Demo Server",
            Command = command,
            Arguments = arguments,
        });
        
        var mcpClient = await McpClientFactory.CreateAsync(clientTransport);

        
    }
}
