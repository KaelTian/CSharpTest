using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using System.Text.Json;


Random _random = new Random();

Console.WriteLine("PLC 数据发布服务启动...");

// 创建托管 MQTT 客户端
var mqttClient = await ManagedMqttClientAsync();

// 模拟 PLC 数据采集并发布
var timer = new System.Timers.Timer(2000); // 每2秒发布一次数据
timer.Elapsed += async (sender, e) => await PublishPlcData(mqttClient);
timer.Start();

Console.WriteLine("按任意键停止服务...");
Console.ReadKey();

timer.Stop();
await mqttClient.StopAsync();

async Task<IManagedMqttClient> ManagedMqttClientAsync()
{
    // 创建 MQTT 客户端选项
    var options = new ManagedMqttClientOptionsBuilder()
        .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
        .WithClientOptions(new MqttClientOptionsBuilder()
        .WithTcpServer("192.168.0.209", 1883)
        .WithClientId($"PLC_Publisher_{Guid.NewGuid()}")
        .WithCleanSession()
        .Build())
        .Build();

    var mqttClient = new MqttFactory().CreateManagedMqttClient();

    // 连接事件处理
    mqttClient.ConnectedAsync += MqttClient_ConnectedAsync;

    mqttClient.DisconnectedAsync += e =>
    {
        Console.WriteLine($"与 MQTT 代理服务器断开连接: {e.Reason}");
        return Task.CompletedTask;
    };

    mqttClient.ConnectingFailedAsync += e =>
    {
        Console.WriteLine($"连接 MQTT 代理服务器失败: {e.Exception}");
        return Task.CompletedTask;
    };

    // 开始连接
    await mqttClient.StartAsync(options);

    return mqttClient;
}

Task MqttClient_ConnectedAsync(MqttClientConnectedEventArgs arg)
{
    Console.WriteLine("已连接到 MQTT 代理服务器");
    return Task.CompletedTask;
}

async Task PublishPlcData(IManagedMqttClient mqttClient)
{
    try
    {
        // 模拟 PLC 数据
        var plcData = new PlcData
        {
            Temperature = Math.Round(20 + _random.NextDouble() * 30, 2), // 20-50°C
            Pressure = Math.Round(1 + _random.NextDouble() * 4, 2),     // 1-5 bar
            Status = _random.NextDouble() > 0.1 ? "运行" : "故障",        // 90% 运行概率
            Timestamp = DateTime.Now
        };

        // 将 PLC 数据序列化为 JSON
        var payload=JsonSerializer.Serialize(plcData);

        // 创建 MQTT 消息
        var message = new MqttApplicationMessageBuilder()
            .WithTopic("plc/data")
            .WithPayload(payload)
            .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce) // 至少一次投递
            .WithRetainFlag(false)
            .Build();

        // 发布消息
        await mqttClient.EnqueueAsync(message);

        Console.WriteLine($"已发布 PLC 数据: {payload} at {DateTime.Now}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"发布 PLC 数据时发生错误: {ex.Message}");
    }
}

// PLC 数据模型
public class PlcData
{
    public double Temperature { get; set; }
    public double Pressure { get; set; }
    public string Status { get; set; } = "运行";
    public DateTime Timestamp { get; set; } = DateTime.Now;
}
