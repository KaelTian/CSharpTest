using SqlSugar;

namespace SpliteTableDBComponentDemo.Entities
{
    [SugarTable("20250101_db4000")]
    public class Db4000
    {
        [SugarColumn(ColumnName = "ID", IsPrimaryKey = true, IsIdentity = true)]
        public long Id { get; set; }

        [SugarColumn(ColumnName = "CreateTime")]
        public DateTime CreateTime { get; set; }

        [SugarColumn(ColumnName = "UpdateTime")]
        public DateTime? UpdateTime { get; set; }

        [SugarColumn(ColumnName = "bit")]
        public bool Bit { get; set; }

        [SugarColumn(ColumnName = "byte")]
        public byte? Byte { get; set; }

        [SugarColumn(ColumnName = "char")]
        public string? Char { get; set; }

        [SugarColumn(ColumnName = "sint")]
        public sbyte? Sint { get; set; }

        [SugarColumn(ColumnName = "int")]
        public short? Int { get; set; }

        [SugarColumn(ColumnName = "dint")]
        public int? Dint { get; set; }

        [SugarColumn(ColumnName = "lint")]
        public long? Lint { get; set; }

        [SugarColumn(ColumnName = "usint")]
        public byte? Usint { get; set; }

        [SugarColumn(ColumnName = "uint")]
        public ushort? Uint { get; set; }

        [SugarColumn(ColumnName = "udint")]
        public uint? Udint { get; set; }

        [SugarColumn(ColumnName = "ulint")]
        public ulong? Ulint { get; set; }

        [SugarColumn(ColumnName = "real")]
        public float? Real { get; set; }

        [SugarColumn(ColumnName = "lreal")]
        public double? Lreal { get; set; }

        [SugarColumn(ColumnName = "word")]
        public string? Word { get; set; }

        [SugarColumn(ColumnName = "dword")]
        public string? Dword { get; set; }

        [SugarColumn(ColumnName = "lword")]
        public string? Lword { get; set; }

        [SugarColumn(ColumnName = "string")]
        public string? String { get; set; }

        [SugarColumn(ColumnName = "date")]
        public DateTime? Date { get; set; }

        [SugarColumn(ColumnName = "time")]
        public int? Time { get; set; }

        [SugarColumn(ColumnName = "datetime")]
        public DateTime? Datetime { get; set; }

        [SugarColumn(ColumnName = "dtl")]
        public DateTime? Dtl { get; set; }

        [SugarColumn(ColumnName = "bit组")]
        public string? BitArray { get; set; }

        [SugarColumn(ColumnName = "byte组")]
        public string? ByteArray { get; set; }

        [SugarColumn(ColumnName = "char组")]
        public string? CharArray { get; set; }

        [SugarColumn(ColumnName = "sint组")]
        public string? SintArray { get; set; }

        [SugarColumn(ColumnName = "int组")]
        public string? IntArray { get; set; }

        [SugarColumn(ColumnName = "dint组")]
        public string? DintArray { get; set; }

        [SugarColumn(ColumnName = "lint组")]
        public string? LintArray { get; set; }

        [SugarColumn(ColumnName = "usint组")]
        public string? UsintArray { get; set; }

        [SugarColumn(ColumnName = "uint组")]
        public string? UintArray { get; set; }

        [SugarColumn(ColumnName = "udint组")]
        public string? UdintArray { get; set; }

        [SugarColumn(ColumnName = "ulint组")]
        public string? UlintArray { get; set; }

        [SugarColumn(ColumnName = "real组")]
        public string? RealArray { get; set; }

        [SugarColumn(ColumnName = "lreal组")]
        public string? LrealArray { get; set; }

        [SugarColumn(ColumnName = "word组")]
        public string? WordArray { get; set; }

        [SugarColumn(ColumnName = "dword组")]
        public string? DwordArray { get; set; }

        [SugarColumn(ColumnName = "lword组")]
        public string? LwordArray { get; set; }

        [SugarColumn(ColumnName = "date组")]
        public string? DateArray { get; set; }

        [SugarColumn(ColumnName = "time组")]
        public string? TimeArray { get; set; }

        [SugarColumn(ColumnName = "datetime组")]
        public string? DatetimeArray { get; set; }

        [SugarColumn(ColumnName = "dtl组")]
        public string? DtlArray { get; set; }

        //// 结构体[0]
        //[SugarColumn(ColumnName = "结构体[0].嵌套[0].bit")]
        //public bool Struct0Nest0Bit { get; set; }
        //[SugarColumn(ColumnName = "结构体[0].嵌套[0].byte")]
        //public sbyte? Struct0Nest0Byte { get; set; }
        //[SugarColumn(ColumnName = "结构体[0].嵌套[0].word")]
        //public string? Struct0Nest0Word { get; set; }
        //[SugarColumn(ColumnName = "结构体[0].嵌套[0].real")]
        //public float? Struct0Nest0Real { get; set; }

        //[SugarColumn(ColumnName = "结构体[0].嵌套[1].bit")]
        //public bool Struct0Nest1Bit { get; set; }
        //[SugarColumn(ColumnName = "结构体[0].嵌套[1].byte")]
        //public sbyte? Struct0Nest1Byte { get; set; }
        //[SugarColumn(ColumnName = "结构体[0].嵌套[1].word")]
        //public string? Struct0Nest1Word { get; set; }
        //[SugarColumn(ColumnName = "结构体[0].嵌套[1].real")]
        //public float? Struct0Nest1Real { get; set; }

        //[SugarColumn(ColumnName = "结构体[0].嵌套[2].bit")]
        //public bool Struct0Nest2Bit { get; set; }
        //[SugarColumn(ColumnName = "结构体[0].嵌套[2].byte")]
        //public sbyte? Struct0Nest2Byte { get; set; }
        //[SugarColumn(ColumnName = "结构体[0].嵌套[2].word")]
        //public string? Struct0Nest2Word { get; set; }
        //[SugarColumn(ColumnName = "结构体[0].嵌套[2].real")]
        //public float? Struct0Nest2Real { get; set; }

        //[SugarColumn(ColumnName = "结构体[0].嵌套[3].bit")]
        //public bool Struct0Nest3Bit { get; set; }
        //[SugarColumn(ColumnName = "结构体[0].嵌套[3].byte")]
        //public sbyte? Struct0Nest3Byte { get; set; }
        //[SugarColumn(ColumnName = "结构体[0].嵌套[3].word")]
        //public string? Struct0Nest3Word { get; set; }
        //[SugarColumn(ColumnName = "结构体[0].嵌套[3].real")]
        //public float? Struct0Nest3Real { get; set; }

        //// 结构体[1]
        //[SugarColumn(ColumnName = "结构体[1].嵌套[0].bit")]
        //public bool Struct1Nest0Bit { get; set; }
        //[SugarColumn(ColumnName = "结构体[1].嵌套[0].byte")]
        //public sbyte? Struct1Nest0Byte { get; set; }
        //[SugarColumn(ColumnName = "结构体[1].嵌套[0].word")]
        //public string? Struct1Nest0Word { get; set; }
        //[SugarColumn(ColumnName = "结构体[1].嵌套[0].real")]
        //public float? Struct1Nest0Real { get; set; }

        //[SugarColumn(ColumnName = "结构体[1].嵌套[1].bit")]
        //public bool Struct1Nest1Bit { get; set; }
        //[SugarColumn(ColumnName = "结构体[1].嵌套[1].byte")]
        //public sbyte? Struct1Nest1Byte { get; set; }
        //[SugarColumn(ColumnName = "结构体[1].嵌套[1].word")]
        //public string? Struct1Nest1Word { get; set; }
        //[SugarColumn(ColumnName = "结构体[1].嵌套[1].real")]
        //public float? Struct1Nest1Real { get; set; }

        //[SugarColumn(ColumnName = "结构体[1].嵌套[2].bit")]
        //public bool Struct1Nest2Bit { get; set; }
        //[SugarColumn(ColumnName = "结构体[1].嵌套[2].byte")]
        //public sbyte? Struct1Nest2Byte { get; set; }
        //[SugarColumn(ColumnName = "结构体[1].嵌套[2].word")]
        //public string? Struct1Nest2Word { get; set; }
        //[SugarColumn(ColumnName = "结构体[1].嵌套[2].real")]
        //public float? Struct1Nest2Real { get; set; }

        //[SugarColumn(ColumnName = "结构体[1].嵌套[3].bit")]
        //public bool Struct1Nest3Bit { get; set; }
        //[SugarColumn(ColumnName = "结构体[1].嵌套[3].byte")]
        //public sbyte? Struct1Nest3Byte { get; set; }
        //[SugarColumn(ColumnName = "结构体[1].嵌套[3].word")]
        //public string? Struct1Nest3Word { get; set; }
        //[SugarColumn(ColumnName = "结构体[1].嵌套[3].real")]
        //public float? Struct1Nest3Real { get; set; }

        //// 结构体[2]
        //[SugarColumn(ColumnName = "结构体[2].嵌套[0].bit")]
        //public bool Struct2Nest0Bit { get; set; }
        //[SugarColumn(ColumnName = "结构体[2].嵌套[0].byte")]
        //public sbyte? Struct2Nest0Byte { get; set; }
        //[SugarColumn(ColumnName = "结构体[2].嵌套[0].word")]
        //public string? Struct2Nest0Word { get; set; }
        //[SugarColumn(ColumnName = "结构体[2].嵌套[0].real")]
        //public float? Struct2Nest0Real { get; set; }

        //[SugarColumn(ColumnName = "结构体[2].嵌套[1].bit")]
        //public bool Struct2Nest1Bit { get; set; }
        //[SugarColumn(ColumnName = "结构体[2].嵌套[1].byte")]
        //public sbyte? Struct2Nest1Byte { get; set; }
        //[SugarColumn(ColumnName = "结构体[2].嵌套[1].word")]
        //public string? Struct2Nest1Word { get; set; }
        //[SugarColumn(ColumnName = "结构体[2].嵌套[1].real")]
        //public float? Struct2Nest1Real { get; set; }

        //[SugarColumn(ColumnName = "结构体[2].嵌套[2].bit")]
        //public bool Struct2Nest2Bit { get; set; }
        //[SugarColumn(ColumnName = "结构体[2].嵌套[2].byte")]
        //public sbyte? Struct2Nest2Byte { get; set; }
        //[SugarColumn(ColumnName = "结构体[2].嵌套[2].word")]
        //public string? Struct2Nest2Word { get; set; }
        //[SugarColumn(ColumnName = "结构体[2].嵌套[2].real")]
        //public float? Struct2Nest2Real { get; set; }

        //[SugarColumn(ColumnName = "结构体[2].嵌套[3].bit")]
        //public bool Struct2Nest3Bit { get; set; }
        //[SugarColumn(ColumnName = "结构体[2].嵌套[3].byte")]
        //public sbyte? Struct2Nest3Byte { get; set; }
        //[SugarColumn(ColumnName = "结构体[2].嵌套[3].word")]
        //public string? Struct2Nest3Word { get; set; }
        //[SugarColumn(ColumnName = "结构体[2].嵌套[3].real")]
        //public float? Struct2Nest3Real { get; set; }

        //// 结构体[3]
        //[SugarColumn(ColumnName = "结构体[3].嵌套[0].bit")]
        //public bool Struct3Nest0Bit { get; set; }
        //[SugarColumn(ColumnName = "结构体[3].嵌套[0].byte")]
        //public sbyte? Struct3Nest0Byte { get; set; }
        //[SugarColumn(ColumnName = "结构体[3].嵌套[0].word")]
        //public string? Struct3Nest0Word { get; set; }
        //[SugarColumn(ColumnName = "结构体[3].嵌套[0].real")]
        //public float? Struct3Nest0Real { get; set; }

        //[SugarColumn(ColumnName = "结构体[3].嵌套[1].bit")]
        //public bool Struct3Nest1Bit { get; set; }
        //[SugarColumn(ColumnName = "结构体[3].嵌套[1].byte")]
        //public sbyte? Struct3Nest1Byte { get; set; }
        //[SugarColumn(ColumnName = "结构体[3].嵌套[1].word")]
        //public string? Struct3Nest1Word { get; set; }
        //[SugarColumn(ColumnName = "结构体[3].嵌套[1].real")]
        //public float? Struct3Nest1Real { get; set; }

        //[SugarColumn(ColumnName = "结构体[3].嵌套[2].bit")]
        //public bool Struct3Nest2Bit { get; set; }
        //[SugarColumn(ColumnName = "结构体[3].嵌套[2].byte")]
        //public sbyte? Struct3Nest2Byte { get; set; }
        //[SugarColumn(ColumnName = "结构体[3].嵌套[2].word")]
        //public string? Struct3Nest2Word { get; set; }
        //[SugarColumn(ColumnName = "结构体[3].嵌套[2].real")]
        //public float? Struct3Nest2Real { get; set; }

        //[SugarColumn(ColumnName = "结构体[3].嵌套[3].bit")]
        //public bool Struct3Nest3Bit { get; set; }
        //[SugarColumn(ColumnName = "结构体[3].嵌套[3].byte")]
        //public sbyte? Struct3Nest3Byte { get; set; }
        //[SugarColumn(ColumnName = "结构体[3].嵌套[3].word")]
        //public string? Struct3Nest3Word { get; set; }
        //[SugarColumn(ColumnName = "结构体[3].嵌套[3].real")]
        //public float? Struct3Nest3Real { get; set; }

        //[SugarColumn(ColumnName = "bit_1")]
        //public bool Bit1 { get; set; }
        //[SugarColumn(ColumnName = "bit_2")]
        //public bool Bit2 { get; set; }
        //[SugarColumn(ColumnName = "bit_3")]
        //public bool Bit3 { get; set; }
        //[SugarColumn(ColumnName = "bit_4")]
        //public bool Bit4 { get; set; }
        //[SugarColumn(ColumnName = "bit_5")]
        //public bool Bit5 { get; set; }
        //[SugarColumn(ColumnName = "bit_6")]
        //public bool Bit6 { get; set; }
        //[SugarColumn(ColumnName = "bit_7")]
        //public bool Bit7 { get; set; }
        //[SugarColumn(ColumnName = "bit_8")]
        //public bool Bit8 { get; set; }
        //[SugarColumn(ColumnName = "bit_9")]
        //public bool Bit9 { get; set; }
        //[SugarColumn(ColumnName = "bit_10")]
        //public bool Bit10 { get; set; }
    }
}
