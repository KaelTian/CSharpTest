using Moq;
using RemoteFileClient.Service.Interfaces;

namespace RemoteFileClient.Test
{
    public class SmbFileClientMockTests
    {
        private readonly Mock<IRemoteFileClient> _mockClient;

        public SmbFileClientMockTests()
        {
            _mockClient = new Mock<IRemoteFileClient>();
        }

        [Fact]
        public async Task FileExistsAsync_ShouldReturnMockValue()
        {
            _mockClient.Setup(x => x.FileExistsAsync(It.IsAny<string>()))
                      .ReturnsAsync(true);

            var result = await _mockClient.Object.FileExistsAsync("anyfile.txt");
            Assert.True(result);
        }

        [Fact]
        public async Task ReadTextFileAsync_ShouldReturnMockContent()
        {
            var expectedContent = "mock content";
            _mockClient.Setup(x => x.ReadTextFileAsync(It.IsAny<string>()))
                      .ReturnsAsync(expectedContent);

            var result = await _mockClient.Object.ReadTextFileAsync("anyfile.txt");
            Assert.Equal(expectedContent, result);
        }

        // 可以继续添加其他接口方法的模拟测试
    }
}
