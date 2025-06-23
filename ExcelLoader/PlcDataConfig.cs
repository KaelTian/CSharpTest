namespace ExcelLoader
{
    public class PlcDataConfig
    {
        /// <summary>
        /// 类型
        /// </summary>
        public string? Category { get; set; }

        /// <summary>
        /// PLC点名称（留空）
        /// </summary>
        public string? PlcPointName { get; set; }

        /// <summary>
        /// 显示名称
        /// </summary>
        public string? DisplayName { get; set; }

        /// <summary>
        /// 是否只读
        /// </summary>
        public bool IsReadOnly { get; set; } = true;
    }
}
