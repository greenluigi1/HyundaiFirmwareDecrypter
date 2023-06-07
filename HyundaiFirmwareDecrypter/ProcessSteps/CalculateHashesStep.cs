using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyundaiFirmwareDecrypter.ProcessSteps
{
    internal class CalculateHashesStep : IProcessStep
    {
        public string Name => "Calculate Hashes";

        public string Description => "Calculate and compile the list of the SHA512 hashes for all of the update files.";

        public string Argument => "-c";
        public int Order => 3;

        public async Task<bool> Execute(ProcessRun run)
        {
            string encryptedUpdateHashListPath = Path.Combine(run.Settings.TempDirectory, Constants.FILE_HASH_LIST_FILENAME_FOR_ENCRYPTED_UPDATE);
            string unencryptedUpdateHashListPath = Path.Combine(run.Settings.TempDirectory, Constants.FILE_HASH_LIST_FILENAME_FOR_UNENCRYPTED_UPDATE);
            string encryptedUpdateSignaturePath = Path.Combine(run.Settings.TempDirectory, Constants.SIGNATURE_FILENAME_FOR_ENCRYPTED_UPDATE);

            List<string> previousFileHashes = (await File.ReadAllLinesAsync(encryptedUpdateHashListPath)).ToList();

            if (File.Exists(unencryptedUpdateHashListPath))
            {
                Console.WriteLine($"Deleting partially completed unencrypted hash list at {Constants.FILE_HASH_LIST_FILENAME_FOR_UNENCRYPTED_UPDATE}");
                File.Delete(unencryptedUpdateHashListPath);
            }

            if (File.Exists(encryptedUpdateSignaturePath))
            {
                Console.WriteLine($"Deleting the unencrypted update hash list signature file {Constants.SIGNATURE_FILENAME_FOR_ENCRYPTED_UPDATE}");
                File.Delete(encryptedUpdateSignaturePath);
            }

            Console.WriteLine($"Creating the unencrypted update file hash list ({Constants.FILE_HASH_LIST_FILENAME_FOR_UNENCRYPTED_UPDATE})");
            List<string> outputHashLines = new List<string>();
            byte[] hashBuffer = new byte[512 / 8];

            foreach (string previousHashLine in previousFileHashes)
            {
                string[] parts = previousHashLine.Split(new char[] { ':' }, 2);

                if (parts.Length != 2)
                {
                    Console.WriteLine($"Invalid hash line: {previousHashLine}. Skipping.");
                    continue;
                }

                string filename = parts[0];

                using (FileStream file = File.OpenRead(Path.Combine(run.Settings.TempDirectory, filename)))
                {
                    byte[] hash = await CryptoUtils.CreateSha512SignatureForBlock(file);

                    string hashString = Convert.ToHexString(hash).ToLowerInvariant();

                    string hashLine = $"{filename}:{hashString}";
                    outputHashLines.Add(hashLine);
                    Console.WriteLine($"Calculated: {hashLine}");
                }
            }

            Console.WriteLine($"Writing the unencrypted update file hash list ({Constants.FILE_HASH_LIST_FILENAME_FOR_UNENCRYPTED_UPDATE})");
            string hashFileContents = string.Join('\n', outputHashLines);
            await File.WriteAllTextAsync(Path.Combine(run.Settings.TempDirectory, Constants.FILE_HASH_LIST_FILENAME_FOR_UNENCRYPTED_UPDATE), hashFileContents);

            Console.WriteLine($"Deleting the encrypted update file hash list ({Constants.FILE_HASH_LIST_FILENAME_FOR_ENCRYPTED_UPDATE})");
            File.Delete(encryptedUpdateHashListPath);

            return true;
        }

        public bool IsCompleted(ProcessRun run)
        {
            string encryptedUpdateHashListPath = Path.Combine(run.Settings.TempDirectory, Constants.FILE_HASH_LIST_FILENAME_FOR_ENCRYPTED_UPDATE);
            string unencryptedUpdateHashListPath = Path.Combine(run.Settings.TempDirectory, Constants.FILE_HASH_LIST_FILENAME_FOR_UNENCRYPTED_UPDATE);
            return File.Exists(unencryptedUpdateHashListPath) && !File.Exists(encryptedUpdateHashListPath);
        }
    }
}
