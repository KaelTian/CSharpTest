using SpliteTableDBComponentDemo.Entities;

namespace SpliteTableDBComponentDemo
{
    public class TestDataGenerator
    {
        private static readonly Random _random = new Random();

        public static List<Db4000> GenerateTestData(int count)
        {
            var testData = new List<Db4000>();

            for (int i = 0; i < count; i++)
            {
                var item = new Db4000
                {
                    // Basic fields
                    Bit = _random.Next(2) == 1,
                    Byte = (byte)_random.Next(0, 256),
                    Char = ((char)_random.Next(65, 91)).ToString(), // A-Z
                    Sint = (sbyte)_random.Next(-128, 128),
                    Int = (short)_random.Next(-32768, 32768),
                    Dint = _random.Next(-2147483648, 2147483647),
                    Lint = (long)(_random.NextDouble() * long.MaxValue),
                    Usint = (byte)_random.Next(0, 256),
                    Uint = (ushort)_random.Next(0, 65536),
                    Udint = (uint)_random.Next(0, int.MaxValue),
                    Ulint = (ulong)(_random.NextDouble() * long.MaxValue),
                    Real = (float)(_random.NextDouble() * 1000),
                    Lreal = _random.NextDouble() * 1000,
                    Word = _random.Next(0, 65536).ToString("X4"),
                    Dword = _random.Next(0, int.MaxValue).ToString("X8"),
                    Lword = ((long)(_random.NextDouble() * long.MaxValue)).ToString("X16"),
                    String = $"TestString_{Guid.NewGuid().ToString().Substring(0, 8)}",
                    Date = DateTime.Now.AddDays(_random.Next(-365, 365)).Date,
                    Time = _random.Next(0, 86400000), // milliseconds in a day
                    Datetime = DateTime.Now.AddDays(_random.Next(-365, 365)),
                    Dtl = DateTime.Now.AddDays(_random.Next(-365, 365)),

                    // Arrays
                    BitArray = $"{_random.Next(2)},{_random.Next(2)},{_random.Next(2)},{_random.Next(2)}",
                    ByteArray = $"{_random.Next(256)},{_random.Next(256)},{_random.Next(256)},{_random.Next(256)}",
                    CharArray = $"{(char)_random.Next(65, 91)},{(char)_random.Next(65, 91)},{(char)_random.Next(65, 91)},{(char)_random.Next(65, 91)}",
                    SintArray = $"{_random.Next(-128, 128)},{_random.Next(-128, 128)},{_random.Next(-128, 128)}",
                    IntArray = $"{_random.Next(-32768, 32768)},{_random.Next(-32768, 32768)},{_random.Next(-32768, 32768)},{_random.Next(-32768, 32768)}",
                    DintArray = $"{_random.Next(-2147483648, 2147483647)},{_random.Next(-2147483648, 2147483647)},{_random.Next(-2147483648, 2147483647)}",
                    LintArray = $"{_random.Next()},{_random.Next()},{_random.Next()}",
                    UsintArray = $"{_random.Next(256)},{_random.Next(256)},{_random.Next(256)}",
                    UintArray = $"{_random.Next(65536)},{_random.Next(65536)},{_random.Next(65536)},{_random.Next(65536)}",
                    UdintArray = $"{_random.Next()},{_random.Next()},{_random.Next()}",
                    UlintArray = $"{_random.Next()},{_random.Next()},{_random.Next()}",
                    RealArray = $"{_random.NextDouble() * 1000},{_random.NextDouble() * 1000},{_random.NextDouble() * 1000},{_random.NextDouble() * 1000}",
                    LrealArray = $"{_random.NextDouble() * 1000},{_random.NextDouble() * 1000},{_random.NextDouble() * 1000}",
                    WordArray = $"{_random.Next(65536).ToString("X4")},{_random.Next(65536).ToString("X4")},{_random.Next(65536).ToString("X4")},{_random.Next(65536).ToString("X4")}",
                    DwordArray = $"{_random.Next().ToString("X8")},{_random.Next().ToString("X8")},{_random.Next().ToString("X8")}",
                    LwordArray = $"{_random.Next().ToString("X16")},{_random.Next().ToString("X16")},{_random.Next().ToString("X16")}",
                    DateArray = $"{DateTime.Now.AddDays(_random.Next(-365, 365)).Date:yyyy-MM-dd},{DateTime.Now.AddDays(_random.Next(-365, 365)).Date:yyyy-MM-dd},{DateTime.Now.AddDays(_random.Next(-365, 365)).Date:yyyy-MM-dd}",
                    TimeArray = $"{_random.Next(86400000)},{_random.Next(86400000)},{_random.Next(86400000)}",
                    DatetimeArray = $"{DateTime.Now.AddDays(_random.Next(-365, 365)):yyyy-MM-dd HH:mm:ss},{DateTime.Now.AddDays(_random.Next(-365, 365)):yyyy-MM-dd HH:mm:ss},{DateTime.Now.AddDays(_random.Next(-365, 365)):yyyy-MM-dd HH:mm:ss}",
                    DtlArray = $"{DateTime.Now.AddDays(_random.Next(-365, 365)):yyyy-MM-dd HH:mm:ss},{DateTime.Now.AddDays(_random.Next(-365, 365)):yyyy-MM-dd HH:mm:ss},{DateTime.Now.AddDays(_random.Next(-365, 365)):yyyy-MM-dd HH:mm:ss}",

                    //// Structure fields - only filling first structure as example
                    //Struct0Nest0Bit = _random.Next(2) == 1,
                    //Struct0Nest0Byte = (sbyte)_random.Next(-128, 128),
                    //Struct0Nest0Word = _random.Next(0, 65536).ToString("X4"),
                    //Struct0Nest0Real = (float)(_random.NextDouble() * 1000),

                    //Struct0Nest1Bit = _random.Next(2) == 1,
                    //Struct0Nest1Byte = (sbyte)_random.Next(-128, 128),
                    //Struct0Nest1Word = _random.Next(0, 65536).ToString("X4"),
                    //Struct0Nest1Real = (float)(_random.NextDouble() * 1000),

                    //Struct0Nest2Bit = _random.Next(2) == 1,
                    //Struct0Nest2Byte = (sbyte)_random.Next(-128, 128),
                    //Struct0Nest2Word = _random.Next(0, 65536).ToString("X4"),
                    //Struct0Nest2Real = (float)(_random.NextDouble() * 1000),

                    //Struct0Nest3Bit = _random.Next(2) == 1,
                    //Struct0Nest3Byte = (sbyte)_random.Next(-128, 128),
                    //Struct0Nest3Word = _random.Next(0, 65536).ToString("X4"),
                    //Struct0Nest3Real = (float)(_random.NextDouble() * 1000),

                    //// Individual bits
                    //Bit1 = _random.Next(2) == 1,
                    //Bit2 = _random.Next(2) == 1,
                    //Bit3 = _random.Next(2) == 1,
                    //Bit4 = _random.Next(2) == 1,
                    //Bit5 = _random.Next(2) == 1,
                    //Bit6 = _random.Next(2) == 1,
                    //Bit7 = _random.Next(2) == 1,
                    //Bit8 = _random.Next(2) == 1,
                    //Bit9 = _random.Next(2) == 1,
                    //Bit10 = _random.Next(2) == 1
                };

                testData.Add(item);
            }

            return testData;
        }
    }
}
