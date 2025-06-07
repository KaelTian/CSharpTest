namespace SqlSugar.Core.Entities
{
    [SplitTable(SplitType.Month)]
    [SugarTable("OrderWithoutIndex_{year}{month}{day}")]
    public class OrderWithoutIndex
    {
        [SugarColumn(IsPrimaryKey = true)]
        public long Id { get; set; }

        [SugarColumn(IsNullable = false, Length = 50)]
        public string? OrderNumber { get; set; }

        [SplitField]
        public DateTime OrderDate { get; set; }

        public decimal Amount { get; set; }

        [SugarColumn(Length = 100)]
        public string? CustomerName { get; set; }

        [SugarColumn(Length = 20)]
        public string? CustomerPhone { get; set; }

        public int Status { get; set; }

        [SugarColumn(Length = 200)]
        public string? Address { get; set; }

        public long CustomerId { get; set; }

        [SugarColumn(Length = 50)]
        public string? PaymentMethod { get; set; }
    }
}
