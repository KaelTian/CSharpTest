namespace TableCreatorTool
{
    // 配置类
    public class TableConfig
    {
        public string Database { get; set; }=string.Empty;
        public string TemplatePath { get; set; } = string.Empty;
        public List<TableGroup> Tables { get; set; } = new List<TableGroup>();
    }

    public class TableGroup
    {
        public string Prefix { get; set; } = string.Empty;
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }
        public string Comment { get; set; } = string.Empty;
    }
}
