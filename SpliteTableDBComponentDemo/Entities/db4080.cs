using SqlSugar;

namespace SpliteTableDBComponentDemo.Entities
{
    [SplitTable(SplitType.Year)]
    [SugarTable("db4080_{year}{month}{day}")]
    public class db4080
    {
        [SugarColumn(IsPrimaryKey = true)]
        public long Id { get; set; }

        [SugarColumn(IsNullable = false, Length = 50)]
        public string? OrderNumber { get; set; }

        [SplitField]
        //[SugarColumn(IndexGroupNameList = new[] { "IX_OrderDate" })]
        public DateTime CreateTime { get; set; }

        public decimal Amount { get; set; }

        [SugarColumn(Length = 100)]
        public string? CustomerName { get; set; }

        [SugarColumn(Length = 20)]
        public string? CustomerPhone { get; set; }

        //[SugarColumn(IndexGroupNameList = new[] { "IX_Status" })]
        public int Status { get; set; }

        [SugarColumn(Length = 200)]
        public string? Address { get; set; }

        //[SugarColumn(IndexGroupNameList = new[] { "IX_CustomerId" })]
        public long CustomerId { get; set; }

        [SugarColumn(Length = 50)]
        public string? PaymentMethod { get; set; }
    }
}
