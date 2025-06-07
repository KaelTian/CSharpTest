using SqlSugar;
using SqlSugar.Core;
using SqlSugar.Core.Configs;
using SqlSugar.Core.Entities;
using SqlSugar.Core.Helpers;
using SqlSugar.Core.Services;
using Xunit.Abstractions;

namespace SqlSugarCore.Test
{
    public class SensorDataTests
    {
        private readonly ITestOutputHelper _output;
        private readonly ISqlSugarClient _db;
        private readonly SensorDataService _dataService;
        private readonly SensorDataQueryService _dataQueryService;

        public SensorDataTests(ITestOutputHelper output)
        {
            _output = output;
            var connectionString = "server=192.168.0.122;Database=SqlSugar5xTest;Uid=root;Pwd=root;AllowLoadLocalInfile=true";
            var dbType = DbType.MySql;
            _db = DbHelper.GetNewDb(connectionString, dbType);
            _dataService = new SensorDataService(_db);
            _dataQueryService = new SensorDataQueryService(_db);
            InitDatabase();
        }

        private void InitDatabase()
        {
            // �������� (SqlSugar��Ҫһ��������ά���ֱ���Ϣ)
            _db.CodeFirst.InitTables<SensorData>();
        }

        [Fact]
        public async Task TestSingleInsert_Async()
        {
            var data = new SensorData
            {
                DeviceId = "Device001",
                Value = 25.6,
                RecordTime = DateTime.Now
            };

            var result = await _dataService.InsertAsync(data);
            Assert.True(result);
        }

        [Fact]
        public async Task TestBulkInsert_Async()
        {
            var random = new Random();
            var dataList = new List<SensorData>();

            // ����3��Ĳ�������
            for (int i = 0; i < 3; i++)
            {
                var date = DateTime.Now.Date.AddDays(-i);
                for (int j = 0; j < 1000; j++)
                {
                    dataList.Add(new SensorData
                    {
                        DeviceId = $"Device{random.Next(1, 10):D3}",
                        Value = random.NextDouble() * 100,
                        RecordTime = date.AddSeconds(random.Next(0, 86400))
                    });
                }
            }

            var result = await _dataService.BulkInsertAsync(dataList);
            Assert.True(result);
        }

        [Fact]
        public async Task TestQueryByDateRange_Async()
        {
            var startDate = DateTime.Now.Date.AddDays(-7);
            var endDate = DateTime.Now.Date;
            var deviceId = "Device10086";

            // �����������
            for (int i = 0; i < 7; i++)
            {
                await _dataService.InsertAsync(new SensorData
                {
                    DeviceId = deviceId,
                    Value = 2 + i,
                    RecordTime = startDate.AddDays(i)
                });
            }

            // ��ѯ
            var results = await _dataQueryService.QueryByDateRangeAsync(startDate, endDate, deviceId);
            Assert.Equal(7, results.Count);
        }

        [Fact]
        public async Task TestStatistics_Async()
        {
            var startDate = DateTime.Now.Date.AddDays(-3);
            var endDate = DateTime.Now.Date;

            // ��ղ�������
            var tableNames = _db.GetTableNames<SensorData>(startDate, endDate);
            foreach (var tableName in tableNames)
            {
                await _db.Deleteable<SensorData>()
                    .AS(tableName)
                    .ExecuteCommandAsync();
            }

            // �����й��ɵĲ��������Ա���֤ͳ�ƽ��
            var dataList = new List<SensorData>();
            for (int i = 0; i < 3; i++)
            {
                var date = startDate.AddDays(i);
                for (int j = 0; j < 10; j++)
                {
                    dataList.Add(new SensorData
                    {
                        DeviceId = "STAT_DEVICE",
                        Value = j + 1,// ֵΪ1-10
                        RecordTime = date.AddHours(j)
                    });
                }
            }

            await _dataService.BulkInsertAsync(dataList);

            // ��ȡͳ��
            var stats = await _dataQueryService.GetStatisticsAsync(startDate, endDate);

            Assert.Equal(5.5, stats["Avg"]);
            Assert.Equal(10, stats["Max"]);
            Assert.Equal(1, stats["Min"]);
            Assert.Equal(30, stats["Count"]);
        }

        [Fact]
        public async Task ContinuousWriteTest_Async()
        {
            var random = new Random();
            var startTime = DateTime.Now;
            var duration = TimeSpan.FromMinutes(2); // ����5����
            var count = 0;

            _output.WriteLine("��ʼ����д�����");

            while (DateTime.Now - startTime < duration)
            {
                var batch = new List<SensorData>();
                var batchSize = random.Next(50, 200);

                for (int i = 0; i < batchSize; i++)
                {
                    batch.Add(new SensorData
                    {
                        DeviceId = $"Device{random.Next(1, 50):D3}",
                        Value = random.NextDouble() * 100,
                        RecordTime = DateTime.Now.AddSeconds(random.Next(-10, 10)) // ��΢��ɢʱ��
                    });
                }

                await _dataService.BulkInsertAsync(batch);
                count += batchSize;

                _output.WriteLine($"��д�� {count} �����ݣ���ǰ���� {batchSize} ��");

                // ������
                Thread.Sleep(random.Next(200, 1000));
            }

            _output.WriteLine($"������ɣ���д�� {count} ������");
            Assert.True(count > 0);
        }

        [Fact]
        public async Task BulkInsertWithNoIndexesTest_Async()
        {
            var startDate = DateTime.Now.AddDays(-10);
            var endDate = DateTime.Now;
            #region ɾ������
            List<TableConfig> tableConfigs = new List<TableConfig>()
            {
                new TableConfig
                {
                     TableNameTemplate="sensor_data_{year}{month}{day}",
                     Indexes=new List<IndexDefinition>
                      {
                          new IndexDefinition
                          {
                               IndexName = "idx_createtime_deviceid",
                               Columns=new string[]
                                {
                                    "CreateTime",
                                    "DeviceId"
                                }
                          }
                      }
                }
            };
            SplitTableHelper.DeleteIndexesForSplitTables(_db, tableConfigs, startDate, endDate, new Action<string, string>((level, message) =>
            {
                _output.WriteLine($"[{level}] {message}");
            }));
            _output.WriteLine("����ɾ�����");
            #endregion
            #region ������ɾ�������
            {
                var random = new Random();
                var count = 0;
                // ��ղ�������
                var tableNames = _db.GetTableNames<SensorData>(startDate, endDate);
                foreach (var tableName in tableNames)
                {
                    await _db.Deleteable<SensorData>()
                        .AS(tableName)
                        .ExecuteCommandAsync();
                }
                // �����������
                var duration = startDate;
                var insertCount = 0;
                long insertCost = 0;
                while (duration < endDate)
                {
                    var batch = new List<SensorData>();
                    //var batchSize = random.Next(20000, 70000);
                    var batchSize = 50_000;
                    var result = ExecutionTimer.MeasureExecutionTime(() =>
                    {
                        for (int i = 0; i < batchSize; i++)
                        {
                            batch.Add(new SensorData
                            {
                                Id = SnowFlakeSingle.instance.NextId(),
                                DeviceId = $"Device-{System.Guid.NewGuid()}",
                                Value = random.NextDouble() * 100,
                                RecordTime = duration.AddMinutes(random.Next(10, 100)).AddSeconds(random.Next(-10, 10)) // ��΢��ɢʱ��
                            });
                        }
                    });

                    _output.WriteLine($"���� {batchSize} �����ݺ�ʱ: {result} ms");

                    result = await ExecutionTimer.MeasureExecutionTimeAsync(async () =>
                    {
                        await _dataService.BulkInsertAsync(batch);
                        insertCount++;
                    });

                    _output.WriteLine($"���� {batchSize} ���������ݿ��ʱ: {result} ms");
                    insertCost += result;
                    count += batchSize;

                    _output.WriteLine($"��д�� {count} �����ݣ���ǰ���� {batchSize} ��");
                    // ������
                    Thread.Sleep(random.Next(200, 1000));
                    duration = duration.AddDays(1);
                }
                _output.WriteLine($"������ɣ���д�� {count} ������,ƽ��ÿ�ű����ʱ�� {insertCost / insertCount} ms");
            }
            #endregion
            #region ��������ѯ
            {
                var duration = startDate;
                var totalCount = 0;
                var searchCount = 0;
                long searchCost = 0;
                while (duration < endDate)
                {
                    bool hasCount = false;
                    long currentCount = 0;
                    var result = await ExecutionTimer.MeasureExecutionTimeAsync(async () =>
                    {
                        var list = await _dataQueryService.QueryByDateRangeAsync(startDate, duration);
                        hasCount = list.Count > 0;
                        currentCount = list.Count;
                        totalCount += list.Count;
                    });
                    if (hasCount)
                    {
                        searchCount++;
                        searchCost += result;
                    }
                    _output.WriteLine($"��ѯ {duration:yyyy-MM-dd} ���ݺ�ʱ: {result} ms, ��ѯ�� {currentCount} ������");
                    duration = duration.AddDays(1);
                    Thread.Sleep(1000); // ÿ����1��
                }
                _output.WriteLine($"��������ѯ��ɣ�����ѯ {searchCount} ������,�ܼ� {totalCount} ������,ƽ����ѯ��ʱ {searchCost / searchCount} ms");
            }
            #endregion
        }

        [Fact]
        public async Task BulkInsertWithIndexesTest_Async()
        {
            var startDate = DateTime.Now.AddDays(-10);
            var endDate = DateTime.Now;
            #region ��������
            List<TableConfig> tableConfigs = new List<TableConfig>()
            {
                new TableConfig
                {
                     TableNameTemplate="sensor_data_{year}{month}{day}",
                     Indexes=new List<IndexDefinition>
                      {
                          new IndexDefinition
                          {
                               IndexName = "idx_createtime_deviceid",
                               Columns=new string[]
                                {
                                    "CreateTime",
                                    "DeviceId"
                                }
                          }
                      }
                }
            };
            SplitTableHelper.CreateIndexesForSplitTables(_db, tableConfigs, startDate, endDate, new Action<string, string>((level, message) =>
            {
                _output.WriteLine($"[{level}] {message}");
            }));
            _output.WriteLine("�����������");
            #endregion
            #region ������ɾ�������
            {
                var random = new Random();
                var count = 0;
                // ��ղ�������
                var tableNames = _db.GetTableNames<SensorData>(startDate, endDate);
                foreach (var tableName in tableNames)
                {
                    await _db.Deleteable<SensorData>()
                        .AS(tableName)
                        .ExecuteCommandAsync();
                }
                // �����������
                var duration = startDate;
                var insertCount = 0;
                long insertCost = 0;
                while (duration < endDate)
                {
                    var batch = new List<SensorData>();
                    //var batchSize = random.Next(20000, 70000);
                    var batchSize = 50_000;
                    var result = ExecutionTimer.MeasureExecutionTime(() =>
                    {
                        for (int i = 0; i < batchSize; i++)
                        {
                            batch.Add(new SensorData
                            {
                                Id = SnowFlakeSingle.instance.NextId(),
                                DeviceId = $"Device-{System.Guid.NewGuid()}",
                                Value = random.NextDouble() * 100,
                                RecordTime = duration.AddMinutes(random.Next(10, 100)).AddSeconds(random.Next(-10, 10)) // ��΢��ɢʱ��
                            });
                        }
                    });

                    _output.WriteLine($"���� {batchSize} �����ݺ�ʱ: {result} ms");

                    result = await ExecutionTimer.MeasureExecutionTimeAsync(async () =>
                    {
                        await _dataService.BulkInsertAsync(batch);
                        insertCount++;
                    });

                    _output.WriteLine($"���� {batchSize} ���������ݿ��ʱ: {result} ms");
                    insertCost += result;
                    count += batchSize;

                    _output.WriteLine($"��д�� {count} �����ݣ���ǰ���� {batchSize} ��");
                    // ������
                    Thread.Sleep(random.Next(200, 1000));
                    duration = duration.AddDays(1);
                }
                _output.WriteLine($"������ɣ���д�� {count} ������,ƽ��ÿ�ű����ʱ�� {insertCost / insertCount} ms");
            }
            #endregion
            #region ��������ѯ
            {
                var duration = startDate;
                var totalCount = 0;
                var searchCount = 0;
                long searchCost = 0;
                while (duration < endDate)
                {
                    bool hasCount = false;
                    long currentCount = 0;
                    var result = await ExecutionTimer.MeasureExecutionTimeAsync(async () =>
                    {
                        var list = await _dataQueryService.QueryByDateRangeAsync(startDate, duration);
                        hasCount = list.Count > 0;
                        currentCount = list.Count;
                        totalCount += list.Count;
                    });
                    if (hasCount)
                    {
                        searchCount++;
                        searchCost += result;
                    }
                    _output.WriteLine($"��ѯ {duration:yyyy-MM-dd} ���ݺ�ʱ: {result} ms, ��ѯ�� {currentCount} ������");
                    duration = duration.AddDays(1);
                    Thread.Sleep(1000); // ÿ����1��
                }
                _output.WriteLine($"��������ѯ��ɣ�����ѯ {searchCount} ������,�ܼ� {totalCount} ������,ƽ����ѯ��ʱ {searchCost / searchCount} ms");
            }
            #endregion
        }
    }
}