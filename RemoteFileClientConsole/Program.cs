using RemoteFileClient.Service.Services;

var factory = new RemoteFileClientFactory();
var smbClient = await factory.CreateSmbClientAsync(uncPath: @"\\192.168.125.209\ShareFolderTest", username: "田赛", password: "ts6yhn7ujm^&*I");

string testFile = @"test.txt";
// 读取文件内容
var strContent = await smbClient.ReadTextFileAsync(testFile);
Console.WriteLine(strContent);

// 写入文件内容
await smbClient.WriteTextFileAsync(testFile, "Hello, World!");
strContent = await smbClient.ReadTextFileAsync(testFile);

Console.WriteLine(strContent);


IDisposable disposableObj = (IDisposable)smbClient;
if (disposableObj != null)
{
    disposableObj.Dispose();
}
else
{
    Console.WriteLine("smbClient is not IDisposable");
}