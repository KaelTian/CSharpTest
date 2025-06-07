using System.Diagnostics;

namespace SqlSugar.Core.Helpers
{
    public static class ExecutionTimer
    {
        /// <summary>
        /// 测量同步方法的执行时间
        /// </summary>
        /// <param name="action">要测量的方法</param>
        /// <returns>执行时间(毫秒)</returns>
        public static long MeasureExecutionTime(Action action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            var stopwatch = Stopwatch.StartNew();
            action();
            stopwatch.Stop();

            return stopwatch.ElapsedMilliseconds;
        }

        /// <summary>
        /// 测量异步方法的执行时间
        /// </summary>
        /// <param name="asyncAction">要测量的异步方法</param>
        /// <returns>执行时间(毫秒)</returns>
        public static async Task<long> MeasureExecutionTimeAsync(Func<Task> asyncAction)
        {
            if (asyncAction == null)
                throw new ArgumentNullException(nameof(asyncAction));

            var stopwatch = Stopwatch.StartNew();
            await asyncAction();
            stopwatch.Stop();

            return stopwatch.ElapsedMilliseconds;
        }

        /// <summary>
        /// 测量同步方法的执行时间并返回结果
        /// </summary>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="func">要测量的方法</param>
        /// <param name="executionTime">输出参数，执行时间(毫秒)</param>
        /// <returns>方法的返回值</returns>
        public static TResult MeasureExecutionTime<TResult>(Func<TResult> func, out long executionTime)
        {
            if (func == null)
                throw new ArgumentNullException(nameof(func));

            var stopwatch = Stopwatch.StartNew();
            TResult result = func();
            stopwatch.Stop();

            executionTime = stopwatch.ElapsedMilliseconds;
            return result;
        }

        /// <summary>
        /// 测量异步方法的执行时间并返回结果
        /// </summary>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="asyncFunc">要测量的异步方法</param>
        /// <returns>包含结果和执行时间的元组</returns>
        public static async Task<(TResult Result, long ElapsedMilliseconds)> MeasureExecutionTimeAsync<TResult>(Func<Task<TResult>> asyncFunc)
        {
            if (asyncFunc == null)
                throw new ArgumentNullException(nameof(asyncFunc));

            var stopwatch = Stopwatch.StartNew();
            TResult result = await asyncFunc();
            stopwatch.Stop();

            return (result, stopwatch.ElapsedMilliseconds);
        }

        /// <summary>
        /// 测量执行时间并输出详细时间信息
        /// </summary>
        /// <param name="action">要测量的方法</param>
        /// <param name="message">自定义消息</param>
        public static void MeasureAndLog(Action action, string? message = null)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            var stopwatch = Stopwatch.StartNew();
            action();
            stopwatch.Stop();

            Console.WriteLine($"{message ?? "Execution time"}: {stopwatch.ElapsedMilliseconds}ms " +
                              $"({stopwatch.ElapsedTicks} ticks)");
        }
    }
}
