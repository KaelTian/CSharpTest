namespace SqlSugar.Core.Configs
{
    /// <summary>
    /// 定义索引和表配置的类
    /// </summary>
    public class TableConfig
    {
        /// <summary>
        /// 表名模板
        /// </summary>
        public string? TableNameTemplate { get; set; }
        /// <summary>
        /// 索引集合
        /// </summary>
        public List<IndexDefinition>? Indexes { get; set; }
    }
    /// <summary>
    /// 索引定义类
    /// </summary>
    public class IndexDefinition
    {
        /// <summary>
        /// 索引名称
        /// </summary>
        public string? IndexName { get; set; }
        /// <summary>
        /// 索引列
        /// </summary>
        public string[]? Columns { get; set; }
        /// <summary>
        /// 是否唯一索引
        /// </summary>
        public bool IsUnique { get; set; } = false;
    }
}
