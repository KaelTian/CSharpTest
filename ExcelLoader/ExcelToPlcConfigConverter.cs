using Newtonsoft.Json;
using OfficeOpenXml;
using System.Text;

namespace ExcelLoader
{
    public class ExcelToPlcConfigConverter
    {
        public List<PlcDataConfig> ParseExcel(string filePath)
        {
            try
            {
                var configs = new List<PlcDataConfig>();

                ExcelPackage.License.SetNonCommercialPersonal("Kael.Tian");
                using (var package = new ExcelPackage(new FileInfo(filePath)))
                {
                    var worksheet = package.Workbook.Worksheets[2];
                    if (worksheet == null)
                    {
                        throw new Exception("Excel文件中没有找到工作表。");
                    }
                    // 从第3行开始读取（跳过表头）
                    int startRow = 3;
                    int rowCount = worksheet.Dimension.Rows;
                    string tempCategory = string.Empty;

                    for (int row = startRow; row <= rowCount; row++)
                    {
                        // 解析各列数据
                        string category = worksheet.Cells[row, 3].Text;       // C列：设备类型
                        string displayName = worksheet.Cells[row, 4].Text;    // D列：DisplayName
                        string permission = worksheet.Cells[row, 6].Text;     // F列：权限

                        if (!string.IsNullOrWhiteSpace(category))
                        {
                            if (string.Equals("/", category))
                            {
                                tempCategory = string.Empty;
                            }
                            else
                            {
                                tempCategory = category;
                            }
                        }

                        // 转换权限为IsReadOnly
                        bool isReadOnly = permission switch
                        {
                            "采+控" => false,
                            "只采不控" => true,
                            _ => true // 默认只读
                        };

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
                                    Category = tempCategory,
                                    DisplayName = subItem,
                                    IsReadOnly = isReadOnly,
                                    PlcPointName = null
                                });
                            }
                        }
                        else
                        {
                            configs.Add(new PlcDataConfig
                            {
                                Category = tempCategory,
                                DisplayName = displayName,
                                IsReadOnly = isReadOnly,
                                PlcPointName = null
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

        public void SaveAsJson(List<PlcDataConfig> configs, string outputPath)
        {
            var json = JsonConvert.SerializeObject(configs, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(outputPath, json);
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
                sb.AppendLine($"public {type}? {name} {{ get; set; }}");
                sb.AppendLine(); // 空一行更清爽

                row++;
            }

            File.WriteAllText("GeneratedProperties.cs", sb.ToString());
            Console.WriteLine("生成完毕！");
        }
    }
}