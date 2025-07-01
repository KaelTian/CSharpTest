//using Newtonsoft.Json;
using Newtonsoft.Json;
using OfficeOpenXml;
using System.Text;
using JsonException = Newtonsoft.Json.JsonException;

namespace ExcelLoader
{
    /// <summary>
    /// 欣奕华上下料单元转换器
    /// </summary>
    public class XinYiHuaLoadingAndUnloadingConverter
    {
        /// <summary>
        /// 从JSON文件转换为C#类
        /// </summary>
        /// <param name="jsonFilePath"></param>
        /// <param name="outputFilePath"></param>
        public void ConvertToClassFromJson(string jsonFilePath, string outputFilePath)
        {
            // 读取JSON文件
            var jsonContent = System.IO.File.ReadAllText(jsonFilePath);

            // 反序列化为对象
            var items = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Item>>(jsonContent)!;

            // 生成C#类代码
            var classCode = GenerateClassCode(items);

            // 保存到输出文件
            System.IO.File.WriteAllText(outputFilePath, classCode);
        }

        private string GenerateClassCode(List<Item> items)
        {
            var sb = new StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine("using System.ComponentModel;"); // 添加DescriptionAttribute的命名空间
            sb.AppendLine("using System.Text.Json.Serialization;");
            sb.AppendLine();
            sb.AppendLine("public class MelsecXinYiHuaLoadingAndUnloading");
            sb.AppendLine("{");
            try
            {
                foreach (var item in items)
                {
                    // 处理属性名
                    string propertyName = GetPropertyName(item.Offset!);

                    // 添加JsonPropertyName标签
                    sb.AppendLine($"    [JsonPropertyName(\"D{item.Offset}\")]");

                    // 添加Description标签
                    sb.AppendLine($"    [Description(\"{item.Name}\")]");

                    // 添加PlcOffset标签
                    sb.AppendLine($"    [PlcOffset(\"{item.Offset}\")]");

                    // 特殊处理STRING类型
                    if (item.Type!.Equals("STRING", StringComparison.OrdinalIgnoreCase))
                    {
                        sb.AppendLine($"    [String(StringType.String, {item.Length})]");
                        sb.AppendLine($"    public string {propertyName} {{ get; set; }} = string.Empty;");
                    }
                    else
                    {
                        sb.AppendLine($"    public {item.Type} {propertyName} {{ get; set; }} = new {item.Type}();");
                    }

                    sb.AppendLine(); // 空一行
                }

                sb.AppendLine("}");

                // 写入生成的C#文件
                string outputFilePath = "MelsecXinYiHuaLoadingAndUnloading.cs";
                File.WriteAllText(outputFilePath, sb.ToString());
                Console.WriteLine($"生成完毕！文件已保存到: {Path.GetFullPath(outputFilePath)}");
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"JSON解析错误: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"发生错误: {ex.Message}");
            }
            return sb.ToString();
        }

        private string GetPropertyName(string offset)
        {
            if (offset.Contains('.'))
            {
                var parts = offset.Split('.');
                return $"D{parts[0]}_{parts[1]}";
            }
            return $"D{offset}";
        }

        /// <summary>
        /// 从Excel文件解析PLC数据映射配置并保存为JSON文件
        /// </summary>
        /// <param name="excelFilePath"></param>
        /// <param name="outputFilePath"></param>
        public void ParsePlcDataMappingFromExcel(string excelFilePath, string outputFilePath)
        {
            try
            {
                var configs = new List<PlcDataMappingConfig>();
                ExcelPackage.License.SetNonCommercialPersonal("Kael.Tian");
                using (var package = new ExcelPackage(new FileInfo(excelFilePath)))
                {
                    var worksheet = package.Workbook.Worksheets["MES交互"];
                    if (worksheet == null)
                    {
                        throw new Exception("Excel文件中没有找到工作表。");
                    }
                    int rowCount = worksheet.Dimension.Rows;
                    int startRow = 2; // 从第二行开始读取
                    for (int row = startRow; row <= rowCount; row++)
                    {
                        // 解析各列数据
                        string plcPointName = worksheet.Cells[row, 2].Text;     // B列：PLC点位名
                        string description = worksheet.Cells[row, 3].Text;     // C列：描述
                        if (string.IsNullOrWhiteSpace(plcPointName)) continue;
                        configs.Add(new PlcDataMappingConfig
                        {
                            Id = plcPointName,
                            PlcTag = plcPointName,
                            Description = description
                        });
                    }
                }
                SaveAsJson(configs, outputFilePath);
            }
            catch (Exception ex)
            {
                throw new Exception($"解析Excel文件失败: {ex.Message}", ex);
            }
        }

        private void SaveAsJson<T>(List<T> configs, string outputPath)
              where T : class
        {
            var json = JsonConvert.SerializeObject(configs, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(outputPath, json);
        }
    }

    public class Item
    {
        public string? Name { get; set; }
        public string? Offset { get; set; }
        public string? Type { get; set; }
        public int? Length { get; set; }
    }
}
