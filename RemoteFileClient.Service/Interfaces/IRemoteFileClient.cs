namespace RemoteFileClient.Service.Interfaces
{
    public interface IRemoteFileClient
    {
        Task<bool> FileExistsAsync(string path);
        Task<string> ReadTextFileAsync(string path);
        Task<byte[]> ReadBinaryFileAsync(string path);
        Task WriteTextFileAsync(string path, string content);
        Task WriteBinaryFileAsync(string path, byte[] content);
        Task<IEnumerable<string>> ListFilesAsync(string directory);
        Task ConnectAsync(); // 新增
        Task DisconnectAsync(); // 新增
        Task<bool> IsConnectedAsync(); // 新增
    }
}
