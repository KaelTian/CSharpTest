using SqlSugar.Core.Entities;

namespace SqlSugar.Core.Services
{
    public class OrderDataService
    {

    }
    public class OrderWithIndexService :
        GenericDataServiceBase<OrderWithIndex>
    {
        public OrderWithIndexService(ISqlSugarClient db)
            : base(db)
        {

        }
    }
    public class OrderWithoutIndexService :
    GenericDataServiceBase<OrderWithoutIndex>
    {
        public OrderWithoutIndexService(ISqlSugarClient db)
            : base(db)
        {

        }
    }
}
