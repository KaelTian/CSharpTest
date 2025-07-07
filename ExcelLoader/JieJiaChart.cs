namespace ExcelLoader
{
    public class JieJiaCharts
    {
        /// <summary>
        /// VG 曲线图
        /// </summary>
        public JieJiaVGChart VG { get; set; } = new JieJiaVGChart();

    }

    public class JieJiaVGChart
    {
        /// <summary>
        /// C1 腔室压力
        /// </summary>
        public List<ChartItem> MES_C1_PG11_Pressure { get; set; } = new List<ChartItem>();
        /// <summary>
        /// C1 管道压力
        /// </summary>
        public List<ChartItem> MES_C1_PG12_Pressure { get; set; } = new List<ChartItem>();
    }

    public class ChartItem
    {
        public DateTime Time { get; set; }

        public float Value { get; set; }
    }
}
