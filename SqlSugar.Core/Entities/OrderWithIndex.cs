namespace SqlSugar.Core.Entities
{
    [SplitTable(SplitType.Month)]
    [SugarTable("OrderWithIndex_{year}{month}{day}")]
    public class OrderWithIndex
    {
        [SugarColumn(IsPrimaryKey = true)]
        public long Id { get; set; }

        [SugarColumn(IsNullable = false, Length = 50)]
        public string? OrderNumber { get; set; }

        [SplitField]
        [SugarColumn(IndexGroupNameList = new[] { "IX_OrderDate" })]
        public DateTime OrderDate { get; set; }

        public decimal Amount { get; set; }

        [SugarColumn(Length = 100)]
        public string? CustomerName { get; set; }

        [SugarColumn(Length = 20)]
        public string? CustomerPhone { get; set; }

        [SugarColumn(IndexGroupNameList = new[] { "IX_Status" })]
        public int Status { get; set; }

        [SugarColumn(Length = 200)]
        public string? Address { get; set; }

        [SugarColumn(IndexGroupNameList = new[] { "IX_CustomerId" })]
        public long CustomerId { get; set; }

        [SugarColumn(Length = 50)]
        public string? PaymentMethod { get; set; }
    }
}
