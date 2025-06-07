namespace SqlSugar.Core.Entities
{
    [SugarTable("Order_{year}{month}{day}")]
    [SplitTable(SplitType.Month)]
    public class Order
    {
        [SugarColumn(IsPrimaryKey = true)]
        public long Id { get; set; }

        [SugarColumn(IsNullable = false, Length = 50)]
        public string? OrderNumber { get; set; }

        [SplitField]
        [SugarColumn(IndexGroupNameList = new[] { "IX_OrderDate" })] // 为OrderDate添加索引
        public DateTime OrderDate { get; set; }

        public decimal Amount { get; set; }

        [SugarColumn(Length = 100)]
        public string? CustomerName { get; set; }

        [SugarColumn(Length = 20)]
        public string? CustomerPhone { get; set; }

        [SugarColumn(IndexGroupNameList = new[] { "IX_Status" })] // 为Status添加索引
        public int Status { get; set; } // 0-未支付, 1-已支付, 2-已发货, 3-已完成, 4-已取消

        [SugarColumn(Length = 200)]
        public string? Address { get; set; }

        [SugarColumn(IndexGroupNameList = new[] { "IX_CustomerId" })] // 为CustomerId添加索引
        public long CustomerId { get; set; }

        [SugarColumn(Length = 50)]
        public string? PaymentMethod { get; set; }
    }
}

