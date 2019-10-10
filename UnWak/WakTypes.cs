namespace UnWak
{
    public struct FiletableEntry
    {
        public int file_offset;
        public int file_size;
        public string filename;
    }

    public class Constants
    {
        public static readonly int wak_key_seed = 0;
        public static readonly int wak_header_IV_seed = 1;
        public static readonly int wak_filetable_IV_seed = 2147483646;
    }
}
