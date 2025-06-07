namespace SpliteTableDBComponentDemo
{
    public class RandomDateTimeGenerator
    {
        private readonly Random _random = new Random();

        public DateTime GenerateRandomDateTime(int year)
        {
            // 检查年份是否有效
            if (year < 1 || year > 9999)
            {
                throw new ArgumentException("年份必须在1到9999之间", nameof(year));
            }

            // 生成随机月份 (1-12)
            int month = _random.Next(1, 13);

            // 计算该月的天数
            int daysInMonth = DateTime.DaysInMonth(year, month);

            // 生成随机日 (1-当月最大天数)
            int day = _random.Next(1, daysInMonth + 1);

            // 生成随机时间 (时、分、秒)
            int hour = _random.Next(0, 24);
            int minute = _random.Next(0, 60);
            int second = _random.Next(0, 60);

            // 创建并返回随机日期时间
            return new DateTime(year, month, day, hour, minute, second);
        }
    }
}
