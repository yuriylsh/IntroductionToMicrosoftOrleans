using System;
using System.Threading.Tasks;
using IoT.GrainClasses;
using IoT.GrainInterfaces;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;

try
{
    await using var host = await StartSilo();
    await using var client = await ConnectClient();
    await DoClientWork(client);
    Console.ReadKey();
    return 0;
}
catch (Exception ex)
{
    Console.WriteLine(ex);
    return 1;
}

static async Task DoClientWork(IGrainFactory client)
{
    var grain = client.GetGrain<IDeviceGrain>(0);
    while (true)
    {
        var temperature = double.Parse(Console.ReadLine()!);
        await grain.SetTemperature(temperature);
    }
}

const string clusterId = "dev";

const string serviceId = "IntroductionToOrleans";

static async Task<ISiloHost> StartSilo()
{
    var builder = new SiloHostBuilder()
        .UseLocalhostClustering()
        .Configure<ClusterOptions>(options =>
        {
            options.ClusterId = clusterId;
            options.ServiceId = serviceId;
        })
        .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(DeviceGrain).Assembly).WithReferences())
        .ConfigureLogging(logging => logging.AddConsole());

    var host = builder.Build();
    await host.StartAsync();
    return host;
}

static async Task<IClusterClient> ConnectClient()
{
    IClusterClient client = new ClientBuilder()
        .UseLocalhostClustering()
        .Configure<ClusterOptions>(options =>
        {
            options.ClusterId = clusterId;
            options.ServiceId = serviceId;
        })
        .ConfigureLogging(logging => logging.AddConsole())
        .Build();

    await client.Connect();
    Console.WriteLine("Client successfully connected to silo host \n");
    return client;
}
