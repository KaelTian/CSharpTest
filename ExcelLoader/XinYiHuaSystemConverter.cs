using Newtonsoft.Json;
using OfficeOpenXml;
using System.Text;

namespace ExcelLoader
{
    /// <summary>
    /// 欣奕华系统转换器
    /// </summary>
    public class XinYiHuaSystemConverter
    {
        /// <summary>
        /// 从Excel文件转换为C#类
        /// </summary>
        /// <param name="jsonFilePath"></param>
        /// <param name="outputFilePath"></param>
        public void ConvertToClassPropertiesFromExcel(string excelFilePath, string outputFilePath, List<string>? sheetList = null)
        {
            ExcelPackage.License.SetNonCommercialPersonal("Kael.Tian");
            using var package = new ExcelPackage(new FileInfo(excelFilePath));
            if (sheetList == null)
            {
                sheetList = new List<string> { "Sheet1" };
            }
            var sb = new StringBuilder();
            foreach (string sheet in sheetList)
            {
                var worksheet = package.Workbook.Worksheets[sheet];
                int row = 2; // 从第二行开始，第一行是标题
                while (true)
                {
                    var plcTag = worksheet.Cells[row, 1].Text.Trim();
                    var type = worksheet.Cells[row, 2].Text.Trim();
                    var desc = worksheet.Cells[row, 3].Text.Trim();
                    if (string.IsNullOrWhiteSpace(plcTag)) break; // 到底了
                    sb.AppendLine($"[JsonPropertyName(\"{plcTag}\")]");
                    sb.AppendLine($"[Description(\"{desc}\")]");
                    sb.AppendLine($"public {type} {plcTag} {{ get; set; }} = new {type}();");
                    sb.AppendLine(); // 空一行更清爽
                    row++;
                }
            }
            File.WriteAllText(outputFilePath, sb.ToString());
            Console.WriteLine("生成完毕！");
        }

        /// <summary>
        /// 从Excel文件解析PLC数据映射配置并保存为JSON文件
        /// </summary>
        /// <param name="excelFilePath"></param>
        /// <param name="outputFilePath"></param>
        public void ParsePlcDataMappingFromExcel(string excelFilePath, string outputFilePath, List<string>? sheetList = null)
        {
            try
            {
                if (sheetList == null)
                {
                    sheetList = new List<string> { "Sheet1" };
                }
                var configs = new List<PlcDataMappingConfig>();
                ExcelPackage.License.SetNonCommercialPersonal("Kael.Tian");
                using (var package = new ExcelPackage(new FileInfo(excelFilePath)))
                {
                    foreach (string sheet in sheetList)
                    {
                        var worksheet = package.Workbook.Worksheets[sheet];
                        if (worksheet == null)
                        {
                            throw new Exception("Excel文件中没有找到工作表。");
                        }
                        int rowCount = worksheet.Dimension.Rows;
                        int startRow = 2; // 从第二行开始读取
                        for (int row = startRow; row <= rowCount; row++)
                        {
                            // 解析各列数据
                            string plcPointName = worksheet.Cells[row, 1].Text;     // A列：PLC点位名
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

        public void GenerateTableScript(string excelPath, string dbName, string tableName, string outputPath, List<string>? sheetList = null)
        {
            if (sheetList == null)
            {
                sheetList = new List<string> { "Sheet1" };
            }
            var sb = new StringBuilder();
            // 表头
            sb.AppendLine($"DROP TABLE IF EXISTS `{dbName}`.`{tableName}`;");
            sb.AppendLine($"CREATE TABLE `{dbName}`.`{tableName}` (");
            sb.AppendLine("  `ID` bigint(0) NOT NULL AUTO_INCREMENT,");
            sb.AppendLine("  `CreateTime` datetime(0) DEFAULT CURRENT_TIMESTAMP COMMENT 'Record creation time',");

            ExcelPackage.License.SetNonCommercialPersonal("Kael.Tian");
            using var package = new ExcelPackage(new FileInfo(excelPath));
            foreach (string sheet in sheetList)
            {
                var worksheet = package.Workbook.Worksheets[sheet];
                int row = 2; // 从第二行开始，第一行是标题
                while (true)
                {
                    var name = worksheet.Cells[row, 1].Text.Trim();
                    var type = worksheet.Cells[row, 2].Text.Trim();
                    var desc = worksheet.Cells[row, 3].Text.Trim();

                    if (string.IsNullOrWhiteSpace(name)) break; // 到底了

                    // 替换特殊字符为下划线
                    var columnName = name.Replace(".", "_").Replace("[", "_").Replace("]", "");

                    sb.AppendLine($"  `{columnName}` {TypeConverter.GetColumnType(type)} COMMENT '{desc?.Replace("'", "''") ?? "REAL field"}',");

                    row++;
                }
            }
            // 表尾
            sb.AppendLine("  PRIMARY KEY (`ID`)");
            sb.AppendLine(") ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;");

            File.WriteAllText(outputPath, sb.ToString());
        }

        public void GenerateDBMappingConfig(string excelPath, string outputPath, List<string>? sheetList = null)
        {
            if (sheetList == null)
            {
                sheetList = new List<string> { "Sheet1" };
            }
            Dictionary<string, string> mappings = new Dictionary<string, string>();
            ExcelPackage.License.SetNonCommercialPersonal("Kael.Tian");
            using var package = new ExcelPackage(new FileInfo(excelPath));
            foreach (string sheet in sheetList)
            {
                var worksheet = package.Workbook.Worksheets[sheet];
                int rowCount = worksheet.Dimension.Rows;
                int startRow = 2; // 从第二行开始，第一行是标题
                for (int row = startRow; row <= rowCount; row++)
                {
                    string plcTag = worksheet.Cells[row, 1].Text;
                    if (string.IsNullOrWhiteSpace(plcTag))
                    {
                        continue; // 跳过空行
                    }
                    mappings.Add(
                        $"OmronXinYiHua#{plcTag}",
                        $"xyh_plc_system#{plcTag}"
                        );
                }
            }
            var json = JsonConvert.SerializeObject(mappings, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(outputPath, json);
            Console.WriteLine($"JSON 文件已保存到: {outputPath}");
        }
    }

    public class SystemItem
    {
        /// <summary>
        /// 点位标签
        /// </summary>
        public string? PlcTag { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public string? Type { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string? Description { get; set; }
    }

}
