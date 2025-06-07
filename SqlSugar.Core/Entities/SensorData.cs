namespace SqlSugar.Core.Entities
{
    // 代表分表的实体类
    [SplitTable(SplitType.Day)] // Specify the split type as "Day"
    [SugarTable("sensor_data_{year}{month}{day}")]
    public class SensorData
    {
        [SugarColumn(IsPrimaryKey = true)]
        public long Id { get; set; }

        public string? DeviceId { get; set; }

        public double Value { get; set; }
        [SplitField]
        public DateTime CreateTime { get; set; }
        [SugarColumn(IsIgnore = true)]
        public DateTime RecordTime
        {
            get { return CreateTime; }
            set { CreateTime = value; }
        }
    }
}
