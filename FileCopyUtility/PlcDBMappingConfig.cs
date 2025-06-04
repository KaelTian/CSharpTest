namespace FileCopyUtility
{
    public class PlcDBMappingConfig
    {
        public int TerminalStartIndex { get; set; }

        public string DbFormat { get; set; } = "FormatDB_{0}";

        public int DBStartIndex { get; set; }
    }
}
