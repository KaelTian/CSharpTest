using CoreLibrary.Class;
using CoreLibrary.Interface;
using log4net;

namespace SpliteTableDBComponentDemo
{
    /// <summary>
    /// 分表数据库组件
    /// </summary>
    public class SpliteTableDBComponent : CoreLibrary.Component.DbComponent
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public SpliteTableDBComponent() : base() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="body">结构体</param>
        /// <param name="line">产线</param>
        /// <param name="workunit">作业单元</param>
        /// <param name="log">日志</param>
        public SpliteTableDBComponent(IComponent body, CoreLibrary.Class.ProductLine line, WorkUnit? workunit, ILog? log) : base(body, line, workunit, log) { }

        public override int Insert<TEntity>(List<TEntity> list, bool isReturnIdentity = false)
        {
            return insert(list, isReturnIdentity);
        }

        private int insert<TEntity>(List<TEntity> list, bool isReturnIdentity = false, int retry = 3)
            where TEntity : class, new()
        {
            int result = -1;
            try
            {
                var insertable = Client.Insertable(list).SplitTable();
                result = insertable.ExecuteCommand();
                //if (isReturnIdentity)
                //{
                //    // 分表操作只能返回雪花ID
                //    long id = insertable.ExecuteReturnSnowflakeId();
                //    result = (int)id;
                //}
                //else
                //{
                //    result = insertable.ExecuteCommand();
                //}
            }
            catch (Exception ex)
            {
                if (retry > 0)
                {
                    Thread.Sleep(1000);
                    result = insert(list, isReturnIdentity, retry - 1);
                }
                else
                {
                    base.Log?.Error($"{((base.WorkUnit == null) ? "公共" : "")}组件: {base.Body?.Name}[{base.Body?.Component}] {ex.ToString()}");
                }
            }
            return result;
        }
    }
}
