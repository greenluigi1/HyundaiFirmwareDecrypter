using Org.BouncyCastle.Crypto.Digests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HyundaiFirmwareDecrypter
{
    internal static class CryptoUtils
    {
        public static async Task DecryptFile(string inputPath, string outputPath, byte[] key, byte[] iv)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;

                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7;

                using (FileStream inputStream = File.OpenRead(inputPath))
                {
                    using (FileStream outputStream = File.OpenWrite(outputPath))
                    {
                        using (CryptoStream cs = new CryptoStream(outputStream, aesAlg.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            await inputStream.CopyToAsync(cs);
                        }
                    }
                }
            }
        }

        public static string CreateSha224SignatureForFile(FileStream fileStream)
        {
            Console.WriteLine($"Calculating Hash");
            long fileSize = fileStream.Length;
            long counter = fileSize / Constants.SHA_BUFFER_SIZE;

            List<byte> totalHashBytes = new List<byte>();

            Span<byte> hashBuffer = stackalloc byte[224 / 8];

            byte[] blockBuffer = new byte[Constants.SHA_BUFFER_SIZE];

            for (long i = 0; i <= counter; i++)
            {
                Console.WriteLine($"Calculating Hash #{i}/{counter}");
                int read = fileStream.ReadAtLeast(blockBuffer, Constants.SHA_BUFFER_SIZE, false);

                Span<byte> block = blockBuffer.AsSpan(0, read);

                CreateSha224SignatureForBlock(block, hashBuffer);

                foreach (byte b in hashBuffer)
                {
                    totalHashBytes.Add(b);
                }
            }

            byte[] totalHashData = totalHashBytes.ToArray();
            CreateSha224SignatureForBlock(totalHashData, hashBuffer);

            File.WriteAllBytes("TotalHashData.hex", totalHashData);
            string finalHash = Convert.ToHexString(hashBuffer).ToLower();
            Console.WriteLine($"Final Calculated Hash: {finalHash}");

            return finalHash;
        }

        public static async Task<byte[]> CreateSha512SignatureForBlock(FileStream file)
        {
            const int BUFFER_SIZE = 1024 * 1024;
            byte[] buffer = new byte[BUFFER_SIZE];
            Sha512Digest digest = new Sha512Digest();

            int totalRead = 0;
            while (totalRead < file.Length)
            {
                int read = await file.ReadAsync(buffer).ConfigureAwait(false);

                digest.BlockUpdate(buffer, 0, read);

                if (read == 0)
                {
                    break;
                }

                totalRead += read;
            }

            byte[] hashBuffer = new byte[512 / 8];
            digest.DoFinal(hashBuffer);

            return hashBuffer;
        }

        public static void CreateSha512SignatureForBlock(ReadOnlySpan<byte> block, Span<byte> hashBuffer)
        {
            Sha512Digest digest = new Sha512Digest();
            digest.BlockUpdate(block);
            digest.DoFinal(hashBuffer);
        }

        public static void CreateSha224SignatureForBlock(ReadOnlySpan<byte> block, Span<byte> hashBuffer)
        {
            Sha224Digest digest = new Sha224Digest();
            digest.BlockUpdate(block);
            digest.DoFinal(hashBuffer);
        }
    }
}
