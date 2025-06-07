using SqlSugar.Core.Entities;

namespace SqlSugar.Core.Services
{
    /// <summary>
    /// 分表插入实现
    /// </summary>
    public class SensorDataService
        :GenericDataServiceBase<SensorData>
    {
        public SensorDataService(ISqlSugarClient db)
            :base(db)
        {
        }
    }
}
