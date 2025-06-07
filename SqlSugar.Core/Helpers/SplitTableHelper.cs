using SqlSugar.Core.Configs;
using System.Reflection;
using System.Text.RegularExpressions;

namespace SqlSugar.Core.Helpers
{
    /// <summary>
    /// 自定义扩展：根据分表类型 + 起止时间生成表名
    /// </summary>
    public static class SplitTableHelper
    {
        public static List<string> GetTableNames<T>(this ISqlSugarClient db, DateTime startDate, DateTime endDate)
            where T : class, new()
        {
            var type = typeof(T);

            var sugarAttr = type.GetCustomAttribute<SugarTable>();
            var splitAttr = type.GetCustomAttribute<SplitTableAttribute>();

            if (sugarAttr == null || splitAttr == null)
                throw new InvalidOperationException($"{type.Name} 缺少 [SugarTable] 或 [SplitTable] 特性");

            var tableTemplate = sugarAttr.TableName;
            var splitType = splitAttr.SplitType;

            var result = new List<string>();

            var cursor = startDate.Date;
            while (cursor <= endDate.Date)
            {
                string tableName = FormatTableName(tableTemplate, cursor, splitType);

                // 多数场景建议判断表是否存在
                if (db.DbMaintenance.IsAnyTable(tableName, false))
                {
                    result.Add(tableName);
                }

                cursor = IncreaseDateTimeBySplitType(cursor, splitType);
            }

            return result;
        }

        private static DateTime IncreaseDateTimeBySplitType(DateTime date, SplitType splitType)
        {
            return splitType switch
            {
                SplitType.Day => date.AddDays(1),
                SplitType.Month => date.AddMonths(1),
                SplitType.Year => date.AddYears(1),
                _ => throw new NotSupportedException()
            };
        }

        private static string FormatTableName(string template, DateTime date, SplitType splitType)
        {
            //// Adjust the template based on the split type
            //switch (splitType)
            //{
            //    case SplitType.Year:
            //        template = Regex.Replace(template, @"(?:_?\{month\}|_?\{day\})", "");
            //        break;
            //    case SplitType.Month:
            //        template = Regex.Replace(template, @"_?\{day\}", "");
            //        break;
            //    case SplitType.Day:
            //        // Keep all placeholders
            //        break;
            //}
            string year = date.Year.ToString("D4");
            string month = date.Month.ToString("D2");
            string day = date.Day.ToString("D2");

            string result = template
                .Replace("{year}", year)
                .Replace("{month}", month)
                .Replace("{day}", day);

            return result;
        }
        /// <summary>
        /// 根据分表配置和起止时间创建索引
        /// </summary>
        /// <param name="db"></param>
        /// <param name="configs"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="logAction"></param>
        public static void CreateIndexesForSplitTables(
            ISqlSugarClient db,
            List<TableConfig> configs,
            DateTime startDate,
            DateTime endDate,
            Action<string, string>? logAction
            )
        {
            CreateOrDeleteIndexes(db, configs, startDate, endDate, logAction, isCreate: true);
        }

        /// <summary>
        /// 根据分表配置和起止时间删除索引
        /// </summary>
        /// <param name="db"></param>
        /// <param name="configs"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="logAction"></param>
        public static void DeleteIndexesForSplitTables(
            ISqlSugarClient db,
            List<TableConfig> configs,
            DateTime startDate,
            DateTime endDate,
            Action<string, string>? logAction
            )
        {
            CreateOrDeleteIndexes(db, configs, startDate, endDate, logAction, isCreate: false);
        }

        private static void CreateOrDeleteIndexes(
            ISqlSugarClient db,
            List<TableConfig> configs,
            DateTime startDate,
            DateTime endDate,
            Action<string, string>? logAction,
            bool isCreate = true
            )
        {
            if (db == null || configs == null || configs.Count == 0)
            {
                logAction?.Invoke("WARN", "没有配置或数据库连接无效");
                return;
            }
            foreach (var config in configs)
            {
                if (string.IsNullOrWhiteSpace(config.TableNameTemplate) ||
                    config.Indexes == null ||
                    config.Indexes.Count == 0)
                    continue;
                var splitType = DetectSplitType(config.TableNameTemplate);
                var cursor = startDate.Date;

                while (cursor <= endDate.Date)
                {
                    string tableName = FormatTableName(config.TableNameTemplate, cursor, splitType);
                    if (!db.DbMaintenance.IsAnyTable(tableName, false))
                    {
                        logAction?.Invoke("WARN", $"表 {tableName} 不存在，跳过索引{(isCreate ? "创建" : "删除")}");
                        cursor = IncreaseDateTimeBySplitType(cursor, splitType); ;
                        continue;
                    }
                    foreach (var index in config.Indexes)
                    {
                        if (index.Columns == null || index.Columns.Length == 0)
                            continue;
                        var indexName = !string.IsNullOrWhiteSpace(index.IndexName) ?
                            $"{tableName}_{index.IndexName}" :
                            $"{tableName}_idx_{string.Join("_", index.Columns)}";
                        if (isCreate)
                        {
                            if (!db.DbMaintenance.IsAnyIndex(indexName))
                            {
                                db.DbMaintenance.CreateIndex(tableName, index.Columns, indexName, isUnique: index.IsUnique);
                                logAction?.Invoke("INFO", $"创建索引 {indexName} 在表 {tableName}");
                            }
                        }
                        else
                        {
                            if (db.DbMaintenance.IsAnyIndex(indexName))
                            {
                                db.DbMaintenance.DropIndex(indexName, tableName);
                                logAction?.Invoke("INFO", $"删除索引 {indexName} 在表 {tableName}");
                            }
                        }
                    }
                    cursor = IncreaseDateTimeBySplitType(cursor, splitType);
                }
            }
        }

        private static SplitType DetectSplitType(string tableNameTemplate)
        {
            if (tableNameTemplate.Contains("{day}"))
            {
                return SplitType.Day;
            }
            if (tableNameTemplate.Contains("{month}"))
            {
                return SplitType.Month;
            }
            if (tableNameTemplate.Contains("{year}"))
            {
                return SplitType.Year;
            }
            throw new ArgumentException("无法从表名模板中检测分表类型", nameof(tableNameTemplate));
        }
    }
}
