namespace SqlSugar.Core.Services
{
    public class GenericDataServiceBase<T>
        where T : class, new()
    {
        private readonly ISqlSugarClient _db;

        public GenericDataServiceBase(ISqlSugarClient db)
        {
            _db = db;
        }

        // 单条插入
        public async Task<bool> InsertAsync(T data)
        {
            return await _db.Insertable(data).SplitTable().ExecuteReturnSnowflakeIdAsync() > 0;
        }

        public async Task<bool> BulkInsertAsync(List<T> dataList)
        {
            if (dataList == null || dataList.Count == 0)
                return false;

            if (dataList?.Count <= 1000)
            {
                var ids = await _db.Insertable(dataList).SplitTable().ExecuteReturnSnowflakeIdListAsync();
                return ids?.Count > 0;
            }
            else
            {
                // 分批插入
                bool success = false;
                var batchSize = 10000;
                try
                {
                    for (int i = 0; i < dataList?.Count; i += batchSize)
                    {
                        var batch = dataList.Skip(i).Take(batchSize).ToList();
                        var count = await _db.Fastest<T>().SplitTable().BulkCopyAsync(batch);
                        if (count < 0)
                        {
                            throw new Exception("插入数据失败");
                        }
                    }
                    success = true;
                }
                catch
                {
                    throw;
                }
                return success;
            }
        }
    }
}
