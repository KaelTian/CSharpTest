using FileCopyUtility;

Console.WriteLine("文件复制工具已启动...");

// 从命令行参数获取输入，或者使用默认值
string sourceFolder = GetArgumentValue(args, 0, @"D:\works\005\TC250501-005\03.开发\Collect\Start\bin\Debug\net8.0\MyTemplateFiles");
string destFolder = GetArgumentValue(args, 1, @"D:\works\005\TC250501-005\03.开发\Collect\Start\bin\Debug\My_Business");
int count = GetIntegerArgument(args, 2, 100);
int startNumber = GetIntegerArgument(args, 3, 1);

Console.WriteLine($"源文件夹: {sourceFolder}");
Console.WriteLine($"目标文件夹: {destFolder}");
Console.WriteLine($"将创建 {count} 个子文件夹");

try
{
    CopyFilesToLineFolders(sourceFolder, destFolder, startNumber, count);
    Console.WriteLine("文件复制完成!");
}
catch (Exception ex)
{
    Console.WriteLine($"发生错误: {ex.Message}");
}

Console.WriteLine("按任意键退出...");
Console.ReadKey();
/// <summary>
/// 从命令行参数获取值，如果参数不存在则使用默认值
/// </summary>
static string GetArgumentValue(string[] args, int index, string defaultValue)
{
    return args.Length > index ? args[index] : defaultValue;
}

/// <summary>
/// 从命令行参数获取整数值，如果参数不存在或无效则使用默认值
/// </summary>
static int GetIntegerArgument(string[] args, int index, int defaultValue)
{
    if (args.Length <= index)
        return defaultValue;

    if (int.TryParse(args[index], out int value))
        return value;

    Console.WriteLine($"警告: 参数 {index} 不是有效的整数，使用默认值 {defaultValue}");
    return defaultValue;
}

/// <summary>
/// 将源文件夹中的文件复制到目标文件夹下的多个LINE子文件夹中
/// </summary>
static void CopyFilesToLineFolders(string sourceFolder, string destFolder, int startNumber, int count)
{
    // 确保源文件夹存在
    if (!Directory.Exists(sourceFolder))
    {
        throw new DirectoryNotFoundException($"源文件夹不存在: {sourceFolder}");
    }

    // 确保目标文件夹存在
    Directory.CreateDirectory(destFolder);

    // 获取源文件夹中的所有文件
    string[] files = Directory.GetFiles(sourceFolder);
    var filteredFiles = files.Where(file => !Path.GetExtension(file).Equals(".pdb", StringComparison.OrdinalIgnoreCase)).ToList();
    if (filteredFiles.Count == 0)
    {
        Console.WriteLine("警告: 源文件夹中没有文件可复制");
        return;
    }

    // 创建指定数量的LINE子文件夹并复制文件
    for (int i = startNumber; i <= count; i++)
    {
        string lineFolder = Path.Combine(destFolder, $"LINE{i}");
        Directory.CreateDirectory(lineFolder);

        Console.WriteLine($"正在处理文件夹: {lineFolder}");

        // 并行复制文件
        Parallel.ForEach(filteredFiles, (file, token) =>
       {
           try
           {
               string fileName = Path.GetFileName(file);
               string destFilePath = Path.Combine(lineFolder, fileName);

               if (string.Equals("PlcDBMapping.json", fileName, StringComparison.OrdinalIgnoreCase))
               {
                   // 如果是 PlcDBMapping.json 文件，使用特定的逻辑处理,通过模板生成json配置文件
                   PlcDBMappingConfig config = LoadPlcDBMappingConfig("PlcDBMappingConfig.json");
                   var templateJson = File.ReadAllText(file);
                   // 替换模板中的占位符
                   var generatedJson = templateJson
                       .Replace("{{terminalname}}", (config.TerminalStartIndex + i - 1).ToString())
                       .Replace("{{dbname}}", string.Format(config.DbFormat, (config.DBStartIndex + i - 1)));
                   File.WriteAllText(destFilePath, generatedJson);
                   Console.WriteLine($"Generated: {destFilePath}");
               }
               else
               {
                   // 使用File.Copy的重载方法，允许覆盖现有文件
                   File.Copy(file, destFilePath, true);
                   Console.WriteLine($"  已复制: {fileName}");
               }
           }
           catch (Exception ex)
           {
               Console.WriteLine($"  复制文件时出错: {ex.Message}");
           }
       });
    }
}

static PlcDBMappingConfig LoadPlcDBMappingConfig(string filePath)
{
    if (!File.Exists(filePath))
    {
        throw new FileNotFoundException($"配置文件不存在: {filePath}");
    }
    string json = File.ReadAllText(filePath);
    return System.Text.Json.JsonSerializer.Deserialize<PlcDBMappingConfig>(json) ?? new PlcDBMappingConfig();
}
