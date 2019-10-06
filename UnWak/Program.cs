using System;
using System.IO;

namespace UnWak
{
    class Program
    {
        static void Main(string[] args)
        {            
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: UnWak [FILE] [<path>]");
                Console.WriteLine("Unpack wak file");
                return;
            }

            byte[] wak_buffer;
            using (FileStream wak_file = File.OpenRead(args[0]))
            {
                wak_buffer = new byte[wak_file.Length];
                wak_file.Read(wak_buffer, 0, (int)wak_file.Length);
            }

            FiletableEntry[] filetable = WakDecryptor.DecryptWak(wak_buffer);
            Console.WriteLine($"Wak file decrypted, found {filetable.Length} files");

            Console.Write("Unpacking...   0%");

            string parent_path = args.Length > 1 ? args[1] : "";
            for (int i = 0; i < filetable.Length; i++)
            {
                FiletableEntry filetable_entry = filetable[i];
                string path = Path.Combine(parent_path, filetable_entry.filename);
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                using (FileStream file = File.Create(path))
                {
                    file.Write(wak_buffer, filetable_entry.file_offset, filetable_entry.file_size);
                }

                if (i % (filetable.Length / 100) == 0)
                {
                    Console.Write($"\b\b\b\b{(100 * i / filetable.Length).ToString().PadLeft(3, ' ')}%");
                }
            }

            Console.WriteLine("\b\b\b\b100%\nDone");
        }
    }
}
