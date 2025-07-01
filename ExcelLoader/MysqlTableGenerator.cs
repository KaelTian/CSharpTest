namespace ExcelLoader
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Text.Json;

    public class MysqlTableGenerator
    {
        public static string GenerateCreateTableScript(List<JJSystemSettingsConfig> configs, string tableName)
        {
            var sb = new StringBuilder();

            // 表头
            sb.AppendLine($"CREATE TABLE `{tableName}` (");
            sb.AppendLine("  `id` int NOT NULL AUTO_INCREMENT,");

            // 为每个配置项生成列
            foreach (var config in configs)
            {
                if (string.IsNullOrEmpty(config.PlcTag))
                    continue;

                // 替换特殊字符为下划线
                var columnName = config.PlcTag.Replace(".", "_").Replace("[", "_").Replace("]", "");

                sb.AppendLine($"  `{columnName}` float DEFAULT NULL COMMENT '{config.Description?.Replace("'", "''") ?? "REAL field"}',");
            }

            // 表尾
            sb.AppendLine("  PRIMARY KEY (`id`)");
            sb.AppendLine(") ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;");

            return sb.ToString();
        }

        public static void GenerateFromJsonFile(string inputJsonPath, string outputSqlPath, string tableName)
        {
            try
            {
                // 读取JSON文件
                var json = File.ReadAllText(inputJsonPath);
                var configs = JsonSerializer.Deserialize<List<JJSystemSettingsConfig>>(json)!;

                // 生成SQL
                var sql = GenerateCreateTableScript(configs, tableName);

                // 写入文件
                File.WriteAllText(outputSqlPath, sql);

                Console.WriteLine($"MySQL建表语句已生成到: {outputSqlPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"错误: {ex.Message}");
            }
        }
    }
}
