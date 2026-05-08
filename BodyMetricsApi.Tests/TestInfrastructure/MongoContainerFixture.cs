using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using System.Net.Sockets;

namespace BodyMetricsApi.Tests.TestInfrastructure;

public sealed class MongoContainerFixture : IAsyncLifetime
{
    private readonly IContainer _container = new ContainerBuilder("mongo:8.0")
        .WithPortBinding(27017, true)
        .Build();

    public string ConnectionString => $"mongodb://localhost:{_container.GetMappedPublicPort(27017)}";

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
        await WaitUntilMongoPortIsReachableAsync();
    }

    public async Task DisposeAsync()
    {
        await _container.DisposeAsync();
    }

    private async Task WaitUntilMongoPortIsReachableAsync()
    {
        var port = _container.GetMappedPublicPort(27017);

        for (var attempt = 0; attempt < 20; attempt++)
        {
            try
            {
                using var tcpClient = new TcpClient();
                await tcpClient.ConnectAsync("localhost", port);
                return;
            }
            catch (SocketException)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(500));
            }
        }

        throw new TimeoutException("MongoDB container port did not become reachable in time.");
    }
}

[CollectionDefinition(Name)]
public sealed class MongoCollectionDefinition : ICollectionFixture<MongoContainerFixture>
{
    public const string Name = "mongo";
}




