using SqlSugar;
using SqlSugar.Core;
using SqlSugar.Core.Entities;
using SqlSugar.Core.Helpers;
using SqlSugar.Core.Services;
using System.Diagnostics;
using Xunit.Abstractions;

namespace SqlSugarCore.Test
{
    public class OrderTest
    {
        private readonly ITestOutputHelper _output;
        private readonly ISqlSugarClient _db;
        private readonly OrderWithIndexService orderWithIndexService;
        private readonly OrderWithoutIndexService orderWithoutIndexService;

        public OrderTest(ITestOutputHelper output)
        {
            _output = output;
            var connectionString = "server=192.168.0.122;Database=SqlSugar5xTest;Uid=root;Pwd=root;AllowLoadLocalInfile=true";
            var dbType = DbType.MySql;
            _db = DbHelper.GetNewDb(connectionString, dbType);
            orderWithIndexService = new OrderWithIndexService(_db);
            orderWithoutIndexService = new OrderWithoutIndexService(_db);
        }

        private async Task GenerateTestDataAsync(int count, DateTime startDate, DateTime endDate)
        {
            try
            {
                var db = _db;
                var random = new Random();
                var dateRange = (endDate - startDate).Days;

                var paymentMethods = new[] { "CreditCard", "PayPal", "Alipay", "WeChatPay", "BankTransfer" };
                var ordersWithIndex = new List<OrderWithIndex>();
                var ordersWithoutIndex = new List<OrderWithoutIndex>();
                for (int i = 0; i < count; i++)
                {
                    var orderDate = startDate.AddDays(random.Next(dateRange));
                    var amount = (decimal)random.Next(10, 10000) + (decimal)random.NextDouble();
                    var customerId = random.Next(1, 100000);
                    var status = random.Next(0, 5);
                    var id = SnowFlakeSingle.Instance.NextId();
                    // 插入带索引的表
                    var orderWithIndex = new OrderWithIndex
                    {
                        Id = id,
                        OrderNumber = $"ORD{DateTime.Now:yyyyMMddHHmmssfff}{random.Next(1000, 9999)}",
                        OrderDate = orderDate,
                        Amount = amount,
                        CustomerName = $"Customer{random.Next(1, 10000)}",
                        CustomerPhone = $"1{random.Next(100000000, 999999999)}",
                        Status = status,
                        Address = $"{random.Next(1, 1000)} Main St, City{random.Next(1, 100)}",
                        CustomerId = customerId,
                        PaymentMethod = paymentMethods[random.Next(0, paymentMethods.Length)]
                    };

                    ordersWithIndex.Add(orderWithIndex);

                    // 插入不带索引的表
                    var orderWithoutIndex = new OrderWithoutIndex
                    {
                        Id = id,
                        OrderNumber = orderWithIndex.OrderNumber,
                        OrderDate = orderDate,
                        Amount = amount,
                        CustomerName = orderWithIndex.CustomerName,
                        CustomerPhone = orderWithIndex.CustomerPhone,
                        Status = status,
                        Address = orderWithIndex.Address,
                        CustomerId = customerId,
                        PaymentMethod = orderWithIndex.PaymentMethod
                    };

                    ordersWithoutIndex.Add(orderWithoutIndex);
                }
                var result = await ExecutionTimer.MeasureExecutionTimeAsync(async () =>
                {
                    // 批量插入带索引的表
                    await orderWithIndexService.BulkInsertAsync(ordersWithIndex);
                });
                _output.WriteLine($"共生成 {count} OrderWithIndex 数据,耗时: {result}ms");
                result = await ExecutionTimer.MeasureExecutionTimeAsync(async () =>
                {
                    // 批量插入不带索引的表
                    await orderWithoutIndexService.BulkInsertAsync(ordersWithoutIndex);
                });
                _output.WriteLine($"共生成 {count} OrderWithoutIndex 数据,耗时: {result}ms");
            }
            catch (Exception ex)
            {
                _output.WriteLine($"生成Order测试数据异常: " + ex.ToString());
            }
        }
        private void TestQueryPerformance()
        {
            var db = _db;
            var random = new Random();

            // 测试数据
            var startDate1 = new DateTime(2022, 1, 1);
            var endDate1 = new DateTime(2022, 12, 31);
            var customerId = random.Next(1, 100000);

            // 测试1: 按日期范围查询
            TestSingleQuery<OrderWithIndex>("带索引表-按日期范围查询",
                q => q.Where(o => o.OrderDate >= startDate1 && o.OrderDate <= endDate1));

            TestSingleQuery<OrderWithoutIndex>("无索引表-按日期范围查询",
                q => q.Where(o => o.OrderDate >= startDate1 && o.OrderDate <= endDate1));

            // 测试2: 按状态查询
            TestSingleQuery<OrderWithIndex>("带索引表-按状态查询",
                q => q.Where(o => o.Status == 2));

            TestSingleQuery<OrderWithoutIndex>("无索引表-按状态查询",
                q => q.Where(o => o.Status == 2));

            // 测试3: 按客户ID查询
            TestSingleQuery<OrderWithIndex>("带索引表-按客户ID查询",
                q => q.Where(o => o.CustomerId == customerId));

            TestSingleQuery<OrderWithoutIndex>("无索引表-按客户ID查询",
                q => q.Where(o => o.CustomerId == customerId));

            // 测试4: 复杂条件查询
            TestSingleQuery<OrderWithIndex>("带索引表-复杂条件查询",
                q => q.Where(o => o.Status == 1 && o.Amount > 1000 && o.PaymentMethod == "CreditCard").OrderBy(o => o.OrderDate));

            TestSingleQuery<OrderWithoutIndex>("无索引表-复杂条件查询",
                q => q.Where(o => o.Status == 1 && o.Amount > 1000 && o.PaymentMethod == "CreditCard").OrderBy(o => o.OrderDate));
        }

        private void TestSingleQuery<T>(string testName, Func<ISugarQueryable<T>, ISugarQueryable<T>> queryAction) where T : class, new()
        {
            var db = _db;
            var sw = Stopwatch.StartNew();
            List<T>? result = null;
            try
            {
                var query = db.Queryable<T>();
                query = queryAction(query);
                result = query.SplitTable().ToList();
            }
            catch (Exception ex)
            {
                _output.WriteLine($"TestSingleQuery 执行异常: {ex}");
            }
            sw.Stop();
            _output.WriteLine($"{testName}({result?.Count}条记录)耗时: {sw.ElapsedMilliseconds}ms");
        }

        private async Task RemoveTestDatasAsync<T>(DateTime startDate, DateTime endDate)
            where T : class, new()
        {
            // 清空测试数据
            var tableNames = _db.GetTableNames<T>(startDate, endDate);
            foreach (var tableName in tableNames)
            {
                await _db.Deleteable<T>()
                    .AS(tableName)
                    .ExecuteCommandAsync();
            }

        }

        private void RemoveOrderTables()
        {
            // 获取所有以 "orderwith" 开头的表
            var tables = _db.DbMaintenance.GetTableInfoList()
                .Where(t => t.Name.StartsWith("orderwith", StringComparison.OrdinalIgnoreCase))
                .ToList();

            // 批量删除
            foreach (var table in tables)
            {
                _db.DbMaintenance.DropTable(table.Name);
                _output.WriteLine($"已删除表: {table.Name}");
            }
        }

        private void RemoveTables<T>()
            where T : class, new()
        {
            try
            {
                var splitTables = _db.SplitHelper<T>().GetTables();
                foreach (var splitTable in splitTables)
                {
                    if (_db.DbMaintenance.IsAnyTable(splitTable.TableName, false))
                    {
                        _db.DbMaintenance.DropTable(splitTable.TableName);
                        _output.WriteLine($"已删除表: {splitTable.TableName}");
                    }
                }
            }
            catch (Exception ex)
            {
                _output.WriteLine($"RemoveTables 执行异常: {ex}");
            }
        }

        private void SelectTest()
        {
            try
            {

                OrderWithIndex orderWithIndex = new OrderWithIndex();
                var name=_db.SplitHelper<OrderWithIndex>().GetTableName(orderWithIndex.OrderDate);

                var list=_db.Queryable<OrderWithIndex>()
                    .SplitTable(tabs=>tabs.InTableNames(name))
                    .ToList();


            }
            catch (Exception ex)
            {
                _output.WriteLine($"SelectTest 执行异常: {ex}");
            }
        }

        [Fact]
        public async Task RunFullTest()
        {
            try
            {
                //RemoveOrderTables();
                RemoveTables<OrderWithIndex>();
                RemoveTables<OrderWithoutIndex>();
                // 1. 生成测试数据 (先生成少量数据测试，如10000条)
                _output.WriteLine("开始生成测试数据...");
                var startDate = new DateTime(2020, 1, 1);
                var endDate = new DateTime(2023, 12, 31);
                await RemoveTestDatasAsync<OrderWithIndex>(startDate, endDate);
                await RemoveTestDatasAsync<OrderWithoutIndex>(startDate, endDate);
                await GenerateTestDataAsync(100000, startDate: startDate, endDate: endDate);
                // 2. 测试查询性能
                _output.WriteLine("\n开始测试查询性能...");
                TestQueryPerformance();
            }
            catch (Exception ex)
            {
                _output.WriteLine($"RunFullTest 执行异常: {ex}");
            }
        }
    }
}
