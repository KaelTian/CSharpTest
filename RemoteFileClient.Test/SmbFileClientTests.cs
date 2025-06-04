using RemoteFileClient.Service.Services;
using Xunit.Sdk;

namespace RemoteFileClient.Test
{
    public class SmbFileClientTests : IDisposable
    {
        private readonly SmbFileClient _client;
        private const string TestUncPath = @"\\192.168.125.209\ShareFolderTest";
        private const string TestUsername = "田赛";
        private const string TestPassword = "ts6yhn7ujm^&*I";
        private const string TestFilePath = "testfile.txt";
        private const string TestDirectory = "remotesubfolder1";

        public SmbFileClientTests()
        {
            _client = new SmbFileClient(TestUncPath, TestUsername, TestPassword);
        }

        public void Dispose()
        {
            _client.Dispose();
        }

        // 辅助方法
        private string GetFullTestPath(string relativePath) => Path.Combine(TestUncPath, relativePath);
        #region 连接相关测试
        [Fact]
        public async Task ConnectAsync_ShouldSetIsConnected_WhenSuccessful()
        {
            // 注意: 这是一个集成测试，需要真实的SMB共享
            try
            {
                await _client.ConnectAsync();
                Assert.True(await _client.IsConnectedAsync());
            }
            catch (System.ComponentModel.Win32Exception)
            {
                // 如果测试环境没有SMB共享，跳过测试
                throw SkipException.ForSkip("SMB share not available for testing");
            }
        }

        [Fact]
        public async Task DisconnectAsync_ShouldResetIsConnected()
        {
            try
            {
                await _client.ConnectAsync();
                await _client.DisconnectAsync();
                Assert.False(await _client.IsConnectedAsync());
            }
            catch (System.ComponentModel.Win32Exception)
            {
                throw SkipException.ForSkip("SMB share not available for testing");
            }
        }

        [Fact]
        public void AnyOperation_WithoutConnect_ShouldThrowInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() => _client.FileExistsAsync(TestFilePath).GetAwaiter().GetResult());
        }
        #endregion
        #region 文件操作测试
        [Fact]
        public async Task FileExistsAsync_ShouldReturnTrue_ForExistingFile()
        {
            try
            {
                await _client.ConnectAsync();

                // 创建测试文件
                var testContent = "test content";
                await _client.WriteTextFileAsync(TestFilePath, testContent);

                var exists = await _client.FileExistsAsync(TestFilePath);
                Assert.True(exists);

                // 清理
                File.Delete(GetFullTestPath(TestFilePath));
            }
            catch (System.ComponentModel.Win32Exception)
            {
                throw SkipException.ForSkip("SMB share not available for testing");
            }
        }

        [Fact]
        public async Task ReadWriteTextFileAsync_ShouldWorkCorrectly()
        {
            try
            {
                await _client.ConnectAsync();

                var testContent = "test content";
                await _client.WriteTextFileAsync(TestFilePath, testContent);

                var readContent = await _client.ReadTextFileAsync(TestFilePath);
                Assert.Equal(testContent, readContent);

                // 清理
                File.Delete(GetFullTestPath(TestFilePath));
            }
            catch (System.ComponentModel.Win32Exception)
            {
                throw SkipException.ForSkip("SMB share not available for testing");
            }
        }

        [Fact]
        public async Task ReadWriteBinaryFileAsync_ShouldWorkCorrectly()
        {
            try
            {
                await _client.ConnectAsync();

                var testContent = new byte[] { 0x01, 0x02, 0x03, 0x04 };
                await _client.WriteBinaryFileAsync(TestFilePath, testContent);

                var readContent = await _client.ReadBinaryFileAsync(TestFilePath);
                Assert.Equal(testContent, readContent);

                // 清理
                File.Delete(GetFullTestPath(TestFilePath));
            }
            catch (System.ComponentModel.Win32Exception)
            {
                throw SkipException.ForSkip("SMB share not available for testing");
            }
        }
        #endregion
        #region 目录操作测试
        [Fact]
        public async Task ListFilesAsync_ShouldReturnFilesInDirectory()
        {
            try
            {
                await _client.ConnectAsync();

                // 创建测试文件和目录
                var file1 = Path.Combine(TestDirectory, "file1.txt");
                var file2 = Path.Combine(TestDirectory, "file2.txt");

                Directory.CreateDirectory(GetFullTestPath(TestDirectory));
                await _client.WriteTextFileAsync(file1, "content1");
                await _client.WriteTextFileAsync(file2, "content2");

                var files = (await _client.ListFilesAsync(TestDirectory)).ToList();

                Assert.Contains(file1, files);
                Assert.Contains(file2, files);
                Assert.Equal(2, files.Count);

                // 清理
                File.Delete(GetFullTestPath(file1));
                File.Delete(GetFullTestPath(file2));
                Directory.Delete(GetFullTestPath(TestDirectory));
            }
            catch (System.ComponentModel.Win32Exception)
            {
                throw SkipException.ForSkip("SMB share not available for testing");
            }
        }
        #endregion
        #region 异常处理测试
        [Fact]
        public async Task ReadTextFileAsync_ShouldThrow_WhenFileNotExists()
        {
            try
            {
                await _client.ConnectAsync();
                var nonExistentFile = "nonexistent.txt";

                await Assert.ThrowsAsync<FileNotFoundException>(() =>
                    _client.ReadTextFileAsync(nonExistentFile));
            }
            catch (System.ComponentModel.Win32Exception)
            {
                throw SkipException.ForSkip("SMB share not available for testing");
            }
        }

        [Fact]
        public void Constructor_ShouldThrow_WhenUncPathInvalid()
        {
            var invalidPath = "invalid/path";
            Assert.Throws<ArgumentException>(() =>
                new SmbFileClient(invalidPath, TestUsername, TestPassword));
        }
        #endregion
    }
}
