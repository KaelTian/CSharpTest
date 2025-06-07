namespace SqlSugar.Core
{
    public class DbHelper
    {
        /// <summary>
        /// Database connection string
        /// 数据库连接字符串
        /// </summary>
        //public readonly static string Connection = "server=192.168.0.122;Database=SqlSugar5xTest;Uid=root;Pwd=root;AllowLoadLocalInfile=true";

        /// <summary>
        /// Get a new SqlSugarClient instance with specific configurations
        /// 获取具有特定配置的新 SqlSugarClient 实例
        /// </summary>
        /// <returns>SqlSugarClient instance</returns>
        public static SqlSugarClient GetNewDb(string connectionString, DbType dbType)
        {
            var db = new SqlSugarClient(new ConnectionConfig()
            {
                IsAutoCloseConnection = true,
                DbType = dbType,
                ConnectionString = connectionString,
                LanguageType = LanguageType.Default//Set language

            },
            it =>
            {
                // Logging SQL statements and parameters before execution
                // 在执行前记录 SQL 语句和参数
                it.Aop.OnLogExecuting = (sql, para) =>
                {
                    // to do: log 替换
                    //Console.WriteLine(UtilMethods.GetNativeSql(sql, para));
                };
            });
            return db;
        }
    }
}
