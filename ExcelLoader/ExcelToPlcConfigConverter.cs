using Newtonsoft.Json;
using OfficeOpenXml;
using System.Text;
using System.Text.Json;

namespace ExcelLoader
{
    public class ExcelToPlcConfigConverter
    {
        /// <summary>
        /// 解析Excel文件，将数据转换为PlcDataConfig列表。
        /// </summary>
        /// <param name="filePath">Excel路径</param>
        /// <param name="targetWorksheet">目标工作表</param>
        /// <param name="startRow">起始行</param>
        /// <param name="plcPointNameColumn">Plc点位列</param>
        /// <param name="displayNameColumn">名称列</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<PlcDataConfig> ParseExcel(
            string filePath,
            string targetWorksheet,
            int startRow = -1,
            int plcPointNameColumn = -1,
            int displayNameColumn = -1)
        {
            try
            {
                var configs = new List<PlcDataConfig>();

                ExcelPackage.License.SetNonCommercialPersonal("Kael.Tian");
                using (var package = new ExcelPackage(new FileInfo(filePath)))
                {
                    var worksheet = package.Workbook.Worksheets[targetWorksheet];
                    if (worksheet == null)
                    {
                        throw new Exception("Excel文件中没有找到工作表。");
                    }
                    if (startRow < 0 || plcPointNameColumn < 0 || displayNameColumn < 0)
                    {
                        throw new Exception("Excel设置的行，列异常。");
                    }
                    int rowCount = worksheet.Dimension.Rows;

                    for (int row = startRow; row <= rowCount; row++)
                    {
                        // 解析各列数据
                        //string category = worksheet.Cells[row, 3].Text;       // C列：设备类型
                        string displayName = worksheet.Cells[row, displayNameColumn].Text;    // D列：DisplayName
                        string plcPointName = worksheet.Cells[row, plcPointNameColumn].Text;     // F列：PLC点位名

                        // 特殊处理带换行的DisplayName
                        if (displayName.Contains("\n"))
                        {
                            var subItems = displayName.Split('\n')
                                .Where(x => !string.IsNullOrWhiteSpace(x))
                                .Select(x => x.Trim());

                            foreach (var subItem in subItems)
                            {
                                configs.Add(new PlcDataConfig
                                {
                                    DisplayName = subItem,
                                    PlcPointName = plcPointName
                                });
                            }
                        }
                        else
                        {
                            configs.Add(new PlcDataConfig
                            {
                                DisplayName = displayName,
                                PlcPointName = plcPointName
                            });
                        }
                    }
                }
                return configs;
            }
            catch (Exception ex)
            {
                throw new Exception($"解析Excel文件失败: {ex.Message}", ex);
            }
        }

        public void SaveAsJson<T>(List<T> configs, string outputPath)
            where T : class
        {
            var json = JsonConvert.SerializeObject(configs, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(outputPath, json);
        }

        public List<PlcDataMappingConfig> GetPlcDataMappingConfigs(
            string filePath,
            string targetWorksheet,
            int startRow = -1,
            int plcPointNameColumn = -1)
        {
            try
            {
                var configs = new List<PlcDataMappingConfig>();

                ExcelPackage.License.SetNonCommercialPersonal("Kael.Tian");
                using (var package = new ExcelPackage(new FileInfo(filePath)))
                {
                    var worksheet = package.Workbook.Worksheets[targetWorksheet];
                    if (worksheet == null)
                    {
                        throw new Exception("Excel文件中没有找到工作表。");
                    }
                    if (startRow < 0 || plcPointNameColumn < 0)
                    {
                        throw new Exception("Excel设置的行，列异常。");
                    }
                    int rowCount = worksheet.Dimension.Rows;

                    for (int row = startRow; row <= rowCount; row++)
                    {
                        // 解析各列数据
                        string plcPointName = worksheet.Cells[row, plcPointNameColumn].Text;     // F列：PLC点位名
                        configs.Add(new PlcDataMappingConfig
                        {
                            Id = plcPointName,
                            PlcTag = plcPointName
                        });
                    }
                }
                return configs;
            }
            catch (Exception ex)
            {
                throw new Exception($"解析Excel文件失败: {ex.Message}", ex);
            }
        }

        public void ParseJieJiaExcel(string filePath)
        {
            ExcelPackage.License.SetNonCommercialPersonal("Kael.Tian");
            using var package = new ExcelPackage(new FileInfo(filePath));
            var worksheet = package.Workbook.Worksheets["变量表"];

            var sb = new StringBuilder();
            int row = 2; // 从第二行开始，第一行是标题
            while (true)
            {
                var name = worksheet.Cells[row, 1].Text.Trim();
                var type = worksheet.Cells[row, 2].Text.Trim();
                var desc = worksheet.Cells[row, 3].Text.Trim();

                if (string.IsNullOrWhiteSpace(name)) break; // 到底了

                sb.AppendLine($"[JsonPropertyName(\"{name}\")]");
                sb.AppendLine($"[Description(\"{desc}\")]");
                sb.AppendLine($"public {type} {name} {{ get; set; }} = new {type}();");
                sb.AppendLine(); // 空一行更清爽

                row++;
            }

            File.WriteAllText("GeneratedProperties.cs", sb.ToString());
            Console.WriteLine("生成完毕！");
        }


        public void GenerateJJSystemSettingsConfig(string filePath, string outputPath)
        {
            Dictionary<string, List<JJSystemSettingsConfig>> jjSystemSettingsContainer = new Dictionary<string, List<JJSystemSettingsConfig>>();
            ExcelPackage.License.SetNonCommercialPersonal("Kael.Tian");
            using var package = new ExcelPackage(new FileInfo(filePath));
            var worksheet = package.Workbook.Worksheets["数据库参数"];
            int rowCount = worksheet.Dimension.Rows;
            int startRow = 2; // 从第二行开始，第一行是标题
            for (int row = startRow; row <= rowCount; row++)
            {
                string id = worksheet.Cells[row, 2].Text;
                string location = worksheet.Cells[row, 7].Text;
                string description = worksheet.Cells[row, 9].Text;
                if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(location))
                {
                    continue; // 跳过空行
                }
                var config = new JJSystemSettingsConfig
                {
                    Id = id,
                    IsReadOnly = true,
                    Category = $"MES_Parameter{location}",
                    Description = description
                };
                if (!jjSystemSettingsContainer.ContainsKey(config.Category))
                {
                    jjSystemSettingsContainer.Add(config.Category, new List<JJSystemSettingsConfig>());
                }
                config.Index = jjSystemSettingsContainer[config.Category].Count;
                config.PlcTag = $"{config.Category}_{config.Index}";
                jjSystemSettingsContainer[config.Category].Add(config);
            }
            // 提取所有 Values 并合并为一个 List
            List<JJSystemSettingsConfig> allSettings = new List<JJSystemSettingsConfig>();
            foreach (var list in jjSystemSettingsContainer.Values)
            {
                allSettings.AddRange(list);
            }

            var json = JsonConvert.SerializeObject(allSettings, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(outputPath, json);
            Console.WriteLine($"JSON 文件已保存到: {outputPath}");
        }

        public void GenerateJJ_TableScript(string excelPath, string dbName, string tableName, string outputPath)
        {
            var sb = new StringBuilder();
            // 表头
            sb.AppendLine($"DROP TABLE IF EXISTS `{dbName}`.`{tableName}`;");
            sb.AppendLine($"CREATE TABLE `{dbName}`.`{tableName}` (");
            sb.AppendLine("  `ID` long NOT NULL AUTO_INCREMENT,");
            sb.AppendLine("  `CreateTime` datetime(0) DEFAULT CURRENT_TIMESTAMP COMMENT 'Record creation time',");

            ExcelPackage.License.SetNonCommercialPersonal("Kael.Tian");
            using var package = new ExcelPackage(new FileInfo(excelPath));
            var worksheet = package.Workbook.Worksheets["变量表"];
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

            // 表尾
            sb.AppendLine("  PRIMARY KEY (`ID`)");
            sb.AppendLine(") ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;");

            File.WriteAllText(outputPath, sb.ToString());
        }

        public void GenerateJJ_Alarm_TableScript(string excelPath, string dbName, string tableName, string outputPath)
        {
            var sb = new StringBuilder();
            // 表头
            sb.AppendLine($"DROP TABLE IF EXISTS `{dbName}`.`{tableName}`;");
            sb.AppendLine($"CREATE TABLE `{tableName}` (");
            sb.AppendLine("  `ID` bigint(0) NOT NULL AUTO_INCREMENT,");
            sb.AppendLine("  `CreateTime` datetime(0) DEFAULT CURRENT_TIMESTAMP COMMENT 'Record creation time',");

            ExcelPackage.License.SetNonCommercialPersonal("Kael.Tian");
            using var package = new ExcelPackage(new FileInfo(excelPath));
            var worksheet = package.Workbook.Worksheets["Sheet1"];
            int row = 2; // 从第二行开始，第一行是标题
            while (true)
            {
                var name = worksheet.Cells[row, 3].Text.Trim();
                //var type = worksheet.Cells[row, 2].Text.Trim();
                var desc = worksheet.Cells[row, 6].Text.Trim();

                if (string.IsNullOrWhiteSpace(name)) break; // 到底了

                // 替换特殊字符为下划线
                var columnName = name.Replace(".", "_").Replace("[", "_").Replace("]", "");

                sb.AppendLine($"  `{columnName}` {TypeConverter.GetColumnType("BOOL")} COMMENT '{desc?.Replace("'", "''") ?? "REAL field"}',");

                row++;
            }

            // 表尾
            sb.AppendLine("  PRIMARY KEY (`ID`)");
            sb.AppendLine(") ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;");

            File.WriteAllText(outputPath, sb.ToString());
        }


        public void GenerateJJAlarmsConfig(string filePath, string outputPath)
        {
            List<JJAlarmConfig> jJAlarms = new List<JJAlarmConfig>();
            ExcelPackage.License.SetNonCommercialPersonal("Kael.Tian");
            using var package = new ExcelPackage(new FileInfo(filePath));
            var worksheet = package.Workbook.Worksheets["Sheet1"];
            int rowCount = worksheet.Dimension.Rows;
            int startRow = 2; // 从第二行开始，第一行是标题
            for (int row = startRow; row <= rowCount; row++)
            {
                //string id = worksheet.Cells[row, 2].Text;
                string id = worksheet.Cells[row, 2].Text;
                string name = worksheet.Cells[row, 3].Text;
                int priority = Convert.ToInt32(worksheet.Cells[row, 4].Text);
                string address = worksheet.Cells[row, 5].Text;
                string description = worksheet.Cells[row, 6].Text;
                if (string.IsNullOrWhiteSpace(name))
                {
                    continue; // 跳过空行
                }
                var config = new JJAlarmConfig
                {
                    Id = id,
                    Name = name,
                    PlcTag = address,
                    Priority = priority,
                    Description = description
                };
                config.Index = jJAlarms.Count;
                jJAlarms.Add(config);
            }
            var json = JsonConvert.SerializeObject(jJAlarms, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(outputPath, json);
            Console.WriteLine($"JSON 文件已保存到: {outputPath}");
        }

        public void GenerateJJAlarmsDBMappingConfig(string filePath, string outputPath)
        {
            Dictionary<string, string> jJAlarms = new Dictionary<string, string>();
            ExcelPackage.License.SetNonCommercialPersonal("Kael.Tian");
            using var package = new ExcelPackage(new FileInfo(filePath));
            var worksheet = package.Workbook.Worksheets["Sheet1"];
            int rowCount = worksheet.Dimension.Rows;
            int startRow = 2; // 从第二行开始，第一行是标题
            for (int row = startRow; row <= rowCount; row++)
            {
                //string id = worksheet.Cells[row, 2].Text;
                string plcTag = worksheet.Cells[row, 3].Text;
                if (string.IsNullOrWhiteSpace(plcTag))
                {
                    continue; // 跳过空行
                }
                jJAlarms.Add(
                    $"OmronJieJia#{plcTag}",
                    $"jj_plc_alarm#{plcTag}"
                    );
            }
            var json = JsonConvert.SerializeObject(jJAlarms, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(outputPath, json);
            Console.WriteLine($"JSON 文件已保存到: {outputPath}");
        }
    }
}