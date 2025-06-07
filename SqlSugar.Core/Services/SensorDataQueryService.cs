using SqlSugar.Core.Entities;

namespace SqlSugar.Core.Services
{
    /// <summary>
    /// 分表查询实现
    /// </summary>
    public class SensorDataQueryService
    {
        private readonly ISqlSugarClient _db;
        public SensorDataQueryService(ISqlSugarClient db)
        {
            _db = db;
        }
        /// <summary>
        /// 跨表查询（日期范围）
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public async Task<List<SensorData>> QueryByDateRangeAsync(DateTime startDate, DateTime endDate, string? deviceId = null)
        {
            var query = _db.Queryable<SensorData>()
                .Where(it => it.CreateTime >= startDate && it.CreateTime <= endDate);
            if (!string.IsNullOrEmpty(deviceId))
            {
                query = query.Where(x => x.DeviceId == deviceId);
            }
            return await query.SplitTable().ToListAsync();
        }
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="deviceId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<(List<SensorData> Data, int Total)> QueryPageAsync(DateTime startDate, DateTime endDate,
            string? deviceId = null, int pageIndex = 1, int pageSize = 20)
        {
            var query = _db.Queryable<SensorData>()
                          .Where(it => it.CreateTime >= startDate && it.CreateTime <= endDate);

            if (!string.IsNullOrEmpty(deviceId))
            {
                query = query.Where(x => x.DeviceId == deviceId);
            }
            query = query.SplitTable();
            var total = await query.CountAsync();
            var data = await query.OrderBy(x => x.CreateTime)
                .ToPageListAsync(pageIndex, pageSize);
            return (data, total);
        }

        public async Task<Dictionary<string, double>> GetStatisticsAsync(DateTime startDate, DateTime endDate)
        {
            var query = _db.Queryable<SensorData>()
                          .Where(it => it.CreateTime >= startDate && it.CreateTime <= endDate)
                          .SplitTable();
            return new Dictionary<string, double>
            {
                ["Avg"] = await query.AvgAsync(x => x.Value),
                ["Max"] = await query.MaxAsync(x => x.Value),
                ["Min"] = await query.MinAsync(x => x.Value),
                ["Count"] = await query.CountAsync()
            };
        }
    }
}
