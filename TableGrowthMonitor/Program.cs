using SqlSugar;
using TableGrowthMonitor;
using DbType = SqlSugar.DbType;

Console.WriteLine("MySQL 表数据增长监控程序启动...");
var config = LoadTableGrowthMonitorConfig("config.json");
// 配置数据库连接
var connectionString = config.ConnectionString;
var tablePrefix = config.TablePrefix; // 默认前缀为 s7_

if (string.IsNullOrWhiteSpace(connectionString) || string.IsNullOrWhiteSpace(tablePrefix))
{
    Console.WriteLine("读取配置失败");
    return;
}
var db = new SqlSugarClient(new ConnectionConfig
{
    ConnectionString = connectionString,
    DbType = DbType.MySql,
    IsAutoCloseConnection = true,
    InitKeyType = InitKeyType.Attribute
});

// 存储上次查询的表记录数
Dictionary<string, long> previousMaxIds = new Dictionary<string, long>();

// 首次获取所有s7_开头的表并初始化计数
var tables = db.DbMaintenance.GetTableInfoList().FindAll(t => t.Name.StartsWith(tablePrefix));
foreach (var table in tables)
{
    long maxId = db.Ado.GetInt($"SELECT MAX(ID) FROM `{table.Name}`");
    previousMaxIds[table.Name] = maxId;
    Console.WriteLine($"表 {table.Name} 最大Id: {maxId}");
}

// 轮询间隔(毫秒)
int pollInterval = 6000;

var cts = new CancellationTokenSource();
var token = cts.Token;

// 在后台监听按键，按下 'q' 退出
Task.Run(() =>
{
    Console.WriteLine("按下 Q 键可随时退出程序...");
    while (true)
    {
        var key = Console.ReadKey(true);
        if (key.Key == ConsoleKey.Q)
        {
            cts.Cancel();
            break;
        }
    }
});

// 主监控循环
while (!token.IsCancellationRequested)
{
    Console.WriteLine($"\n[{DateTime.Now}] 开始新一轮检测...");

    bool allTablesNormal = true;
    int exceptionTableCount = 0;
    foreach (var tableName in previousMaxIds.Keys)
    {
        try
        {
            long currentMaxId = db.Ado.GetInt($"SELECT MAX(ID) FROM `{tableName}`");
            long previousMaxId = previousMaxIds[tableName];

            if (currentMaxId == previousMaxId)
            {
                Console.WriteLine($"警告: 表 {tableName} 记录数未增长! 当前最大ID: {currentMaxId}");
                allTablesNormal = false;
                ++exceptionTableCount;
            }

            previousMaxIds[tableName] = currentMaxId;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"查询表 {tableName} 时出错: {ex.Message}");
        }
    }

    if (allTablesNormal)
    {
        Console.WriteLine("所有表数据增长正常");
    }
    else
    {
        Console.WriteLine($"异常增长数据表数量:{exceptionTableCount}");
    }
    if (exceptionTableCount != 0)
    {
        exceptionTableCount = 0;
    }
    Thread.Sleep(pollInterval);
}

Console.WriteLine("监控已手动终止，程序退出。");

static TableGrowthMonitorConfig LoadTableGrowthMonitorConfig(string filePath)
{
    if (!File.Exists(filePath))
    {
        throw new FileNotFoundException($"配置文件不存在: {filePath}");
    }
    string json = File.ReadAllText(filePath);
    return System.Text.Json.JsonSerializer.Deserialize<TableGrowthMonitorConfig>(json) ?? new TableGrowthMonitorConfig();
}