﻿using System;
using System.Buffers.Binary;
using System.Security.Cryptography;
using System.Text;

namespace UnWak
{
    public class WakDecryptor
    {
        /// <summary>
        /// Decrypts the specified wak file in place
        /// </summary>
        /// <param name="wak_file">The wak file to decrypt</param>
        /// <returns></returns>
        static public FiletableEntry[] DecryptWak(byte[] wak_file)
        {
            return DecryptWak(wak_file, wak_file);
        }

        /// <summary>
        /// Decrypts the specified wak file and copies the resulting data to the output byte array
        /// </summary>
        /// <param name="wak_file">The wak file to decrypt</param>
        /// <param name="output">The output to which to write the decrypted file</param>
        /// <returns></returns>
        static public FiletableEntry[] DecryptWak(byte[] wak_file, byte[] output)
        {
            byte[] key = new byte[16];
            byte[] IV = new byte[16];
            GenerateIV(key, Constants.wak_key_seed);
            GenerateIV(IV, Constants.wak_header_IV_seed);

            Aes128CounterMode aes = new Aes128CounterMode(IV);
            ICryptoTransform ict = aes.CreateDecryptor(key, null);
            ict.TransformBlock(wak_file, 0, 16, output, 0); // decrypt header

            int num_files = BinaryPrimitives.ReadInt32LittleEndian(new ReadOnlySpan<byte>(output, 4, 4));
            int files_offset = BinaryPrimitives.ReadInt32LittleEndian(new ReadOnlySpan<byte>(output, 8, 4));

            FiletableEntry[] fileTable = new FiletableEntry[num_files];

            GenerateIV(IV, Constants.wak_filetable_IV_seed);
            ict.TransformBlock(wak_file, 16, files_offset - 16, output, 16); // decrypt filetable

            int entry_offset = 16;
            for (int i = 0; i < num_files; i++)
            {
                int file_offset = BinaryPrimitives.ReadInt32LittleEndian(new ReadOnlySpan<byte>(output, entry_offset, 4));
                int file_size = BinaryPrimitives.ReadInt32LittleEndian(new ReadOnlySpan<byte>(output, entry_offset + 4, 4));
                int filename_length = BinaryPrimitives.ReadInt32LittleEndian(new ReadOnlySpan<byte>(output, entry_offset + 8, 4));
                string filename = Encoding.UTF8.GetString(new ReadOnlySpan<byte>(output, entry_offset + 12, filename_length));

                GenerateIV(IV, i); // file IV
                ict.TransformBlock(wak_file, file_offset, file_size, output, file_offset); // decrypt file

                fileTable[i].file_offset = file_offset;
                fileTable[i].file_size = file_size;
                fileTable[i].filename = filename;

                entry_offset += 12 + filename_length;
            }

            return fileTable;
        }

        static private void GenerateIV(Span<byte> IV, int iseed)
        {
            iseed += 0x165ec8f;
            WakRng prng = new WakRng(iseed < 0 ? iseed + 4294967296.0 : iseed);
            for (int i = 0; i < 4; i++)
            {
                BinaryPrimitives.WriteInt32LittleEndian(IV.Slice(i * 4, 4), (int)(prng.Next() * -2147483648.0));
            }
        }
    }
}
