namespace ExcelLoader
{
    /// <summary>
    /// PLC数据UI映射配置
    /// </summary>
    public class PlcDataMappingConfig
    {
        /// <summary>
        /// 显示Id
        /// </summary>
        public string? Id { get; set; }
        /// <summary>
        /// PLC标签名
        /// </summary>
        public string? PlcTag { get; set; }
        /// <summary>
        /// 是否只读
        /// </summary>
        public bool IsReadOnly { get; set; } = true;
        /// <summary>
        /// 描述
        /// </summary>
        public string? Description { get; set; }
    }
    public class JJSystemSettingsConfig : PlcDataMappingConfig
    {
        /// <summary>
        /// 种类(
        /// MES_ParameterSys;
        /// MES_ParameterC1;
        /// MES_ParameterC345;
        /// MES_ParameterC7;
        /// MES_ParameterAL;
        /// MES_ParameterCAM;
        /// MES_ParameterMFC;
        /// MES_ParameterPS;
        /// MES_ParameterLS1;
        /// MES_ParameterLS2;
        /// MES_ParameterULS;)
        /// </summary>
        public string? Category { get; set; }
        /// <summary>
        /// 数组索引
        /// </summary>
        public int Index { get; set; }
    }

    public class JJAlarmConfig : PlcDataMappingConfig
    {
        /// <summary>
        /// 数组索引
        /// </summary>
        public int Index { get; set; }

        public string? Name { get; set; }

        public int Priority { get; set; }
    }
}
