//using Newtonsoft.Json;
using Newtonsoft.Json;
using OfficeOpenXml;
using System.Text;
using System.Xml.Linq;
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
            var items = Newtonsoft.Json.JsonConvert.DeserializeObject<List<LoadingAndUnloadingItem>>(jsonContent)!;

            // 生成C#类代码
            var classCode = GenerateClassCode(items);

            // 保存到输出文件
            System.IO.File.WriteAllText(outputFilePath, classCode);
        }

        private string GenerateClassCode(List<LoadingAndUnloadingItem> items)
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

        public void GenerateTableScript(string jsonFilePath, string dbName, string tableName, string outputPath)
        {

            // 读取JSON文件
            var jsonContent = System.IO.File.ReadAllText(jsonFilePath);

            // 反序列化为对象
            var items = Newtonsoft.Json.JsonConvert.DeserializeObject<List<LoadingAndUnloadingItem>>(jsonContent)!;

            var sb = new StringBuilder();
            // 表头
            sb.AppendLine($"DROP TABLE IF EXISTS `{dbName}`.`{tableName}`;");
            sb.AppendLine($"CREATE TABLE `{dbName}`.`{tableName}` (");
            sb.AppendLine("  `ID` bigint(0) NOT NULL AUTO_INCREMENT,");
            sb.AppendLine("  `CreateTime` datetime(0) DEFAULT CURRENT_TIMESTAMP COMMENT 'Record creation time',");

            foreach (var item in items)
            {

                var name = item.Offset;
                var type = item.Type!;
                var desc = item.Name;

                if (string.IsNullOrWhiteSpace(name)) break; // 到底了

                // 替换特殊字符为下划线
                var columnName = $"D{name.Replace(".", "_").Replace("[", "_").Replace("]", "")}";

                sb.AppendLine($"  `{columnName}` {TypeConverter.GetColumnType(type, item.Length)} COMMENT '{desc?.Replace("'", "''") ?? "REAL field"}',");

            }


            // 表尾
            sb.AppendLine("  PRIMARY KEY (`ID`)");
            sb.AppendLine(") ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;");

            File.WriteAllText(outputPath, sb.ToString());
        }

        public void GenerateDBMappingConfig(string jsonFilePath, string outputPath)
        {
            Dictionary<string, string> mappings = new Dictionary<string, string>();
            // 读取JSON文件
            var jsonContent = System.IO.File.ReadAllText(jsonFilePath);

            // 反序列化为对象
            var items = Newtonsoft.Json.JsonConvert.DeserializeObject<List<LoadingAndUnloadingItem>>(jsonContent)!;
            foreach (var item in items)
            {
                string plcTag = item.Offset!;
                if (string.IsNullOrWhiteSpace(plcTag))
                {
                    continue; // 跳过空行
                }
                // 替换特殊字符为下划线
                var columnName = plcTag.Replace(".", "_").Replace("[", "_").Replace("]", "");
                mappings.Add(
                    $"MelsecXinYiHuaLoadingAndUnloading#D{plcTag}",
                    $"xyh_plc_loading_and_unloading#D{columnName}"
                    );
            }
            var json = JsonConvert.SerializeObject(mappings, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(outputPath, json);
            Console.WriteLine($"JSON 文件已保存到: {outputPath}");
        }

        private void SaveAsJson<T>(List<T> configs, string outputPath)
              where T : class
        {
            var json = JsonConvert.SerializeObject(configs, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(outputPath, json);
        }
    }

    public class LoadingAndUnloadingItem
    {
        /// <summary>
        /// 名称 （D100.0）
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// 偏移量 100.0
        /// </summary>
        public string? Offset { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public string? Type { get; set; }
        /// <summary>
        /// 长度（仅STRING类型需要）
        /// </summary>
        public int? Length { get; set; }
    }
}
