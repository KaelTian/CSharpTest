using SqlSugar;
using System.Text.Json;

namespace TableCreatorTool
{
    public class TableCreator
    {
        public void CreateTables(string configPath)
        {
            // 1. 加载配置文件
            var config = LoadConfig(configPath);

            // 2. 读取SQL模板
            string template = File.ReadAllText(config.TemplatePath);

            List<string> createSqls = new List<string>();
            // 3. 遍历所有表配置
            foreach (var tableGroup in config.Tables)
            {
                for (int i = tableGroup.StartIndex; i <= tableGroup.EndIndex; i++)
                {
                    string tableName = $"{tableGroup.Prefix}{i}";

                    // 4. 替换占位符
                    string createSql = template
                        .Replace("{Database}", config.Database)
                        .Replace("{TableName}", tableName)
                        .Replace("{Comment}", tableGroup.Comment);

                    // 5. 缓存SQL
                    createSqls.Add(createSql);
                }
            }

            // 初始化数据库连接
            var db = new SqlSugarClient(new ConnectionConfig
            {
                ConnectionString = "server=192.168.0.122;user id=root;password=root;database=005_mes;charset=utf8;sslMode=None;pooling=true;minpoolsize=1;maxpoolsize=1024;ConnectionLifetime=30;DefaultCommandTimeout=600;",
                DbType = DbType.MySql,
                IsAutoCloseConnection = true,
                InitKeyType = InitKeyType.Attribute
            });

            // 6. 执行SQL
            ExecuteCreateTableScripts(db, createSqls);
        }

        private TableConfig LoadConfig(string path)
        {
            string json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<TableConfig>(json) ?? new TableConfig();
        }

        //private void ExecuteSql(string sql)
        //{
        //    using (var connection = new MySqlConnection("your_connection_string"))
        //    {
        //        connection.Open();
        //        using (var command = new MySqlCommand(sql, connection))
        //        {
        //            command.ExecuteNonQuery();
        //        }
        //    }
        //}

        /// <summary>
        /// 批量执行创建表语句
        /// </summary>
        /// <param name="db">数据库连接对象</param>
        /// <param name="sqlList">创建表语句列表</param>
        private void ExecuteCreateTableScripts(SqlSugarClient db, List<string> sqlList)
        {
            // 开始事务（可选，确保所有表创建操作要么全部成功，要么全部失败）
            db.Ado.BeginTran();

            try
            {
                foreach (var sql in sqlList)
                {
                    // 执行单条创建表语句
                    db.Ado.ExecuteCommand(sql);
                    Console.WriteLine("执行创建表语句成功：\n" + sql);
                }

                // 提交事务
                db.Ado.CommitTran();
            }
            catch (Exception ex)
            {
                // 回滚事务
                db.Ado.RollbackTran();
                Console.WriteLine("执行创建表语句失败：" + ex.Message);
                throw; // 重新抛出异常以便上层处理
            }
        }
    }
}
