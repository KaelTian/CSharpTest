// 读取模板文件
using GenerateTemplateJson;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

try
{
    var config = JsonConvert.DeserializeObject<LineGenRule>(File.ReadAllText("line_config.json"));
    var template = File.ReadAllText("template.json");
    var configs = new List<JObject>();

    for (int i = config!.StartId; i < config.StartId + config.Count; i++)
    {
        var rule = config.DbTableRule!.FirstOrDefault(r => i >= r.Range![0] && i <= r.Range[1]);
        if (rule == null) continue;

        string dbSuffix = (rule.DbBlockStartIndex + i - 1).ToString();

        var data = new Dictionary<string, object>
        {
            ["id"] = i,
            ["name"] = string.Format(config.LineNameFormat!, i),
            ["lineip"] = config.LineIp!,
            ["line"] = $"{config.LineNamePrefix}{i}",
            //["dbtable"] = $"{rule.Prefix}{dbSuffix}",
            ["dbtable"] = $"{dbSuffix}",
            ["plcip"] = rule.Plcip!,
            ["dbblock"] = $"{dbSuffix}"
        };

        var jValue = TemplateHelper.FillTemplateWithTypedValues(template, data);
        configs.Add(jValue);

    }
    // 输出为一整个 JSON 数组
    string resultJson = JsonConvert.SerializeObject(configs, Formatting.Indented);
    File.WriteAllText("generated_configs.json", resultJson);
    Console.WriteLine("生成完成！路径: generated_configs.json");
}
catch (FileNotFoundException ex)
{
    Console.WriteLine($"文件未找到: {ex.Message}");
}
catch (JsonException ex)
{
    Console.WriteLine($"JSON 解析错误: {ex.Message}");
}
catch (Exception ex)
{
    Console.WriteLine($"发生错误: {ex.Message}");
}
Console.WriteLine("Press Any key to exit.");
Console.Read();

class LineGenRule
{
    public int Count { get; set; }
    public int StartId { get; set; }
    public string? LineIp { get; set; }
    public string? LineNamePrefix { get; set; }
    public string? LineNameFormat { get; set; }
    public List<DbTableRule>? DbTableRule { get; set; }

}

class DbTableRule
{
    public int[]? Range { get; set; }  // [start, end]
    public string? Prefix { get; set; }
    public string? Plcip { get; set; }
    public string? DbBlockPrefix { get; set; }
    public int DbBlockStartIndex { get; set; }
}