using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using System.Net.Sockets;

namespace BodyMetricsApi.Tests.TestInfrastructure;

public sealed class AzuriteContainerFixture : IAsyncLifetime
{
    private const ushort BlobPort = 10000;
    private const string AccountName = "bodymetrics";
    private const string AccountKey = "MDEyMzQ1Njc4OWFiY2RlZjAxMjM0NTY3ODlhYmNkZWY=";

    private readonly IContainer _container = new ContainerBuilder("mcr.microsoft.com/azure-storage/azurite:3.35.0")
        .WithEnvironment("AZURITE_ACCOUNTS", $"{AccountName}:{AccountKey}")
        .WithCommand("azurite-blob", "--blobHost", "0.0.0.0", "--blobPort", BlobPort.ToString(), "--location", "/data", "--skipApiVersionCheck")
        .WithPortBinding(BlobPort, true)
        .Build();

    public string ConnectionString
    {
        get
        {
            var mappedPort = _container.GetMappedPublicPort(BlobPort);
            return $"DefaultEndpointsProtocol=http;AccountName={AccountName};AccountKey={AccountKey};BlobEndpoint=http://127.0.0.1:{mappedPort}/{AccountName};";
        }
    }

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
        await WaitUntilBlobPortIsReachableAsync();
    }

    public async Task DisposeAsync()
    {
        await _container.DisposeAsync();
    }

    private async Task WaitUntilBlobPortIsReachableAsync()
    {
        var port = _container.GetMappedPublicPort(BlobPort);

        for (var attempt = 0; attempt < 20; attempt++)
        {
            try
            {
                using var tcpClient = new TcpClient();
                await tcpClient.ConnectAsync("127.0.0.1", port);
                return;
            }
            catch (SocketException)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(500));
            }
        }

        throw new TimeoutException("Azurite container port did not become reachable in time.");
    }
}



