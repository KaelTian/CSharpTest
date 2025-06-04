namespace RemoteFileClient.Service.Interfaces
{
    public interface IRemoteFileClientFactory
    {
        Task<IRemoteFileClient> CreateSmbClientAsync(string uncPath, string username, string password);
        //Task<IRemoteFileClient> CreateFtpClientAsync(string server, NetworkCredential credential);
    }
}
