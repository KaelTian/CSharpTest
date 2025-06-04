using RemoteFileClient.Service.Interfaces;

namespace RemoteFileClient.Service.Services
{
    public class RemoteFileClientFactory : IRemoteFileClientFactory
    {
        public async Task<IRemoteFileClient> CreateSmbClientAsync(string uncPath, string username, string password)
        {
            var client = new SmbFileClient(uncPath, username, password);
            await client.ConnectAsync();
            return client;
        }

        // 其他工厂方法
    }
}
