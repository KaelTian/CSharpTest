using RemoteFileClient.Service.Interfaces;
using System.Net;
using System.Runtime.InteropServices;

namespace RemoteFileClient.Service.Services
{
    public class SmbFileClient : IRemoteFileClient, IDisposable
    {
        private readonly string _uncPath;
        private readonly NetworkCredential _credentials;
        private bool _isConnected = false;

        public SmbFileClient(string uncPath, string username, string password)
        {
            if (!uncPath.StartsWith(@"\\"))
            {
                throw new ArgumentException("UNC path must start with \\\\");
            }
            if (!uncPath.EndsWith(@"\"))
            {
                uncPath += @"\";
            }
            _uncPath = uncPath;
            _credentials = new NetworkCredential(username, password);
        }

        public async Task<bool> FileExistsAsync(string path)
        {
            EnsureConnected();
            return await Task.Run(() => File.Exists(GetFullPath(path)));
        }

        public async Task<IEnumerable<string>> ListFilesAsync(string directory)
        {
            EnsureConnected();
            return await Task.Run(() =>
            {
                var fullPath = GetFullPath(directory);
                return Directory.EnumerateFiles(fullPath).Select(f => Path.GetRelativePath(_uncPath, f));
            });
        }

        public async Task<byte[]> ReadBinaryFileAsync(string path)
        {
            EnsureConnected();
            return await File.ReadAllBytesAsync(GetFullPath(path));
        }

        public async Task<string> ReadTextFileAsync(string path)
        {
            EnsureConnected();
            return await File.ReadAllTextAsync(GetFullPath(path));
        }

        public async Task WriteBinaryFileAsync(string path, byte[] content)
        {
            EnsureConnected();
            await File.WriteAllBytesAsync(GetFullPath(path), content);
        }

        public async Task WriteTextFileAsync(string path, string content)
        {
            EnsureConnected();
            await File.WriteAllTextAsync(GetFullPath(path), content);
        }

        public async Task ConnectAsync()
        {
            await Task.Run(() =>
            {
                var netResource = new NetResource
                {
                    Scope = ResourceScope.GlobalNetwork,
                    ResourceType = ResourceType.Disk,
                    DisplayType = ResourceDisplaytype.Share,
                    Usage = ResourceUsage.Connectable,
                    RemoteName = _uncPath.TrimEnd('\\'),
                    LocalName = null,
                    Provider = null
                };
                int result = WNetAddConnection2(netResource, _credentials.Password, _credentials.UserName, 0);
                if (result != 0)
                {
                    throw new System.ComponentModel.Win32Exception($"Failed to connect to SMB share. Error code: {result}");
                }
                _isConnected = true;
            });
        }

        public async Task DisconnectAsync()
        {
            await Task.Run(() =>
             {
                 Dispose();
             });
        }

        public Task<bool> IsConnectedAsync() => Task.FromResult(_isConnected);

        public void Dispose()
        {
            if (_isConnected)
            {
                WNetCancelConnection2(_uncPath.TrimEnd('\\'), 0, true);
                _isConnected = false;
            }
        }

        private string GetFullPath(string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
            {
                throw new ArgumentNullException(
                    nameof(relativePath),
                    "相对路径不能为 null。请提供有效的相对路径字符串。"
                );
            }
            return Path.Combine(_uncPath, relativePath);
        }

        private void EnsureConnected()
        {
            if (!_isConnected)
            {
                throw new InvalidOperationException("Not connected to SMB share. Call ConnectAsync first.");
            }
        }


        [DllImport("mpr.dll")]
        private static extern int WNetAddConnection2(NetResource netResource, string password, string username, int flags);

        [DllImport("mpr.dll")]
        private static extern int WNetCancelConnection2(string name, int flags, bool force);

        [StructLayout(LayoutKind.Sequential)]
        private class NetResource
        {
            public ResourceScope Scope;
            public ResourceType ResourceType;
            public ResourceDisplaytype DisplayType;
            public ResourceUsage Usage;
            public string? LocalName;
            public string? RemoteName;
            public string? Comment;
            public string? Provider;
        }

        private enum ResourceScope : int
        {
            Connected = 1,
            GlobalNetwork,
            Remembered,
            Recent,
            Context
        }

        private enum ResourceType : int
        {
            Any = 0,
            Disk = 1,
            Print = 2,
            Reserved = 8,
        }

        private enum ResourceDisplaytype : int
        {
            Generic = 0x0,
            Domain = 0x01,
            Server = 0x02,
            Share = 0x03,
            File = 0x04,
            Group = 0x05,
            Network = 0x06,
            Root = 0x07,
            Shareadmin = 0x08,
            Directory = 0x09,
            Tree = 0x0a,
            Ndscontainer = 0x0b
        }

        private enum ResourceUsage : int
        {
            Connectable = 0x00000001,
            Container = 0x00000002,
            Nolocaldevice = 0x00000004,
            Sibling = 0x00000008,
            Attached = 0x00000010,
            All = Connectable | Container | Attached,
        }
    }
}
