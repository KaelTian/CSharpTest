using Newtonsoft.Json;
using OfficeOpenXml;
using System.Text;

namespace ExcelLoader
{
    public class JJEQIOConfig
    {
        /// <summary>
        /// 对应数据索引
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// PLC索引点位ID
        /// </summary>
        public string? Id { get; set; }
        /// <summary>
        /// PLC标签名
        /// </summary>
        public string? PlcTag { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string? Description { get; set; }
    }
    /// <summary>
    /// 捷佳EQ_IO点位集合配置
    /// </summary>
    public class JieJiaEQIOConverter
    {
        /// <summary>
        /// 从Excel文件解析PLC索引点位映射配置并保存为JSON文件
        /// </summary>
        /// <param name="excelFilePath"></param>
        /// <param name="outputFilePath"></param>
        public void ParseIndexMappingFromExcel(string excelFilePath, string outputFilePath)
        {
            try
            {
                var configs = GetJieJiaEQIOConfigs(excelFilePath);
                SaveAsJson(configs, outputFilePath);
            }
            catch (Exception ex)
            {
                throw new Exception($"解析Excel文件失败: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 创建建表的SQL语句
        /// </summary>
        /// <param name="excelPath"></param>
        /// <param name="dbName"></param>
        /// <param name="tableName"></param>
        /// <param name="outputPath"></param>
        public void GenerateTableScript(string excelPath, string dbName, string tableName, string outputPath)
        {
            var sb = new StringBuilder();
            // 表头
            sb.AppendLine($"DROP TABLE IF EXISTS `{dbName}`.`{tableName}`;");
            sb.AppendLine($"CREATE TABLE `{dbName}`.`{tableName}` (");
            sb.AppendLine("  `ID` bigint(0) NOT NULL AUTO_INCREMENT,");
            sb.AppendLine("  `CreateTime` datetime(0) DEFAULT CURRENT_TIMESTAMP COMMENT 'Record creation time',");

            var configs = GetJieJiaEQIOConfigs(excelPath);

            foreach (var config in configs)
            {
                var name = config.PlcTag;
                var type = "BOOL";
                var desc = config.Description;

                if (string.IsNullOrWhiteSpace(name)) break; // 到底了

                // 替换特殊字符为下划线
                var columnName = name.Replace(".", "_").Replace("[", "_").Replace("]", "");

                sb.AppendLine($"  `{columnName}` {TypeConverter.GetColumnType(type)} COMMENT '{desc?.Replace("'", "''") ?? "BOOL field"}',");
            }

            // 表尾
            sb.AppendLine("  PRIMARY KEY (`ID`)");
            sb.AppendLine(") ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;");

            File.WriteAllText(outputPath, sb.ToString());
        }

        private List<JJEQIOConfig> GetJieJiaEQIOConfigs(string excelFilePath)
        {
            var configs = new List<JJEQIOConfig>();
            ExcelPackage.License.SetNonCommercialPersonal("Kael.Tian");
            using (var package = new ExcelPackage(new FileInfo(excelFilePath)))
            {
                var worksheet = package.Workbook.Worksheets["Sheet1"];
                if (worksheet == null)
                {
                    throw new Exception("Excel文件中没有找到工作表。");
                }
                int rowCount = worksheet.Dimension.Rows;
                int startRow = 2; // 从第二行开始读取
                for (int row = startRow; row <= rowCount; row++)
                {
                    // 解析各列数据
                    string address = worksheet.Cells[row, 2].Text;
                    string description = worksheet.Cells[row, 3].Text;
                    string englishName = worksheet.Cells[row, 4].Text;
                    string plcTag = GeneratePlcTagName(englishName);
                    int index = ParseAddressIndex(address);
                    if (string.IsNullOrWhiteSpace(plcTag)) continue;
                    configs.Add(new JJEQIOConfig
                    {
                        Id = plcTag,
                        PlcTag = plcTag,
                        Description = description,
                        Index = index
                    });
                }
            }
            configs = configs.OrderBy(config => config.Index).ToList();
            return configs;
        }

        private void SaveAsJson<T>(List<T> configs, string outputPath)
              where T : class
        {
            var json = JsonConvert.SerializeObject(configs, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(outputPath, json);
        }

        private int ParseAddressIndex(string address)
        {
            // 解析类似"EQ_Process[0]"的地址获取索引
            int start = address.IndexOf('[') + 1;
            int end = address.IndexOf(']');
            string indexStr = address.Substring(start, end - start);
            return int.Parse(indexStr);
        }

        private string GeneratePlcTagName(string englishName)
        {
            // 替换空格为下划线
            string tagName = englishName.Replace(" ", "_");

            // 处理括号内容
            if (tagName.Contains('('))
            {
                int start = tagName.IndexOf('(');
                int end = tagName.IndexOf(')');
                string inside = tagName.Substring(start + 1, end - start - 1);

                // 移除非字母数字字符
                string cleanInside = new string(inside.Where(c => char.IsLetterOrDigit(c)).ToArray());

                tagName = tagName.Remove(start, end - start + 1);
                tagName = tagName.Insert(start, "_" + cleanInside);
            }

            return tagName;
        }
    }
}
