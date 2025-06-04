using MQTTnet;
using System.Text;

const int ChunkSize = 10 * 1024;// 每片10kb
string broker = "192.168.0.209";
string topicBase = "filetransfer";

//string testTopic = "testtopic/#";
//using var client = await ConnectClientAsync(broker);
//// 订阅测试主题
//client.ApplicationMessageReceivedAsync += e =>
//{
//    var topic = e.ApplicationMessage.Topic;
//    var message = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
//    Console.WriteLine($"[Receiver] Received message {message}.");
//    return Task.CompletedTask;
//};
//var response = await client.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic(testTopic).Build());
//Console.WriteLine("[Receiver] Subscribed and waiting for chunks...");
//foreach (var item in response.Items)
//{
//    Console.WriteLine($"Topic: {item.TopicFilter.Topic}, QoS: {item.ResultCode}");
//}
//Console.ReadLine();

if (args.Length > 0 && args[0] == "Send")
{
    await SendFileAsync("test_send.txt", broker, topicBase);
}
else
{
    await ReceiveFileAsync("test_receive.txt", broker, topicBase);
}
Console.WriteLine("Press enter to exit.");
Console.ReadLine();
async Task<IMqttClient> ConnectClientAsync(string broker)
{
    var factory = new MqttClientFactory();
    var client = factory.CreateMqttClient();
    MqttClientOptions options = new MqttClientOptionsBuilder()
        //.WithClientId("FileTransferClient")
        .WithTcpServer(broker)
        .Build();
    await client.ConnectAsync(options, CancellationToken.None);
    return client;
}

async Task SendFileAsync(string filePath, string broker, string topicBase)
{
    using var client = await ConnectClientAsync(broker);
    byte[] fileBytes = File.ReadAllBytes(filePath);
    var totalChunks = (int)Math.Ceiling((double)fileBytes.Length / ChunkSize);

    Console.WriteLine($"[Sender] Sending file '{filePath}' in {totalChunks} chunks...");

    for (int i = 0; i < totalChunks; i++)
    {
        byte[] chunk = fileBytes.Skip(i * ChunkSize).Take(ChunkSize).ToArray();
        string topic = $"{topicBase}/part/{i}";
        string playload = Convert.ToBase64String(chunk);

        var applicationMessage = new MqttApplicationMessageBuilder()
            .WithTopic(topic)
            .WithPayload(playload)
            .Build();

        await client.PublishAsync(applicationMessage, CancellationToken.None);
    }

    await client.PublishAsync(
        new MqttApplicationMessageBuilder()
            .WithTopic($"{topicBase}/part/end")
            .WithPayload(totalChunks.ToString())
            .Build(),
        CancellationToken.None
    );
    Console.WriteLine("[Sender] File send completed.");
}

async Task ReceiveFileAsync(string outputPath, string broker, string topicBase)
{
    using var client = await ConnectClientAsync(broker);
    var chunks = new SortedDictionary<int, byte[]>();
    int? expectedChunks = null;

    client.ApplicationMessageReceivedAsync += e =>
    {
        var topic = e.ApplicationMessage.Topic;
        if (topic.EndsWith("/end"))
        {
            expectedChunks = int.Parse(Encoding.UTF8.GetString(e.ApplicationMessage.Payload));
            Console.WriteLine($"[Receiver] Expecting {expectedChunks} chunks.");
            //return Task.CompletedTask;
        }
        else
        {
            var match = System.Text.RegularExpressions.Regex.Match(topic, @"/part/(\d+)$");
            if (match.Success)
            {
                int index = int.Parse(match.Groups[1].Value);
                byte[] chunk = Convert.FromBase64String(Encoding.UTF8.GetString(e.ApplicationMessage.Payload));
                chunks[index] = chunk;
                Console.WriteLine($"[Receiver] Received chunk {index}.");
            };
        }
        return Task.CompletedTask;
    };
    var mqttSubscribleOptions = new MqttClientSubscribeOptionsBuilder()
        .WithTopicFilter(new MqttTopicFilterBuilder().WithTopic($"{topicBase}/part/#").Build())
        .Build();
    await client.SubscribeAsync(mqttSubscribleOptions, CancellationToken.None);
    Console.WriteLine("[Receiver] Subscribed and waiting for chunks...");

    // 等待接收所有片段
    while (expectedChunks == null || chunks.Count < expectedChunks)
    {
        await Task.Delay(500);
    }

    // 合并所有片段
    using (var fs = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
    {
        foreach (var part in chunks.OrderBy(k => k.Key))
        {
            fs.Write(part.Value, 0, part.Value.Length);
        }
    }

    Console.WriteLine($"[Receiver] File reconstructed and saved to '{outputPath}'");
}