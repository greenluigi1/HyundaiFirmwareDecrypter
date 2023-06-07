using ICSharpCode.SharpZipLib.Zip;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyundaiFirmwareDecrypter.ProcessSteps
{
    internal class DecryptStep : IProcessStep
    {
        public string Name => "Decrypt";

        public string Description => "Decrypt all of the extracted files.";

        public string Argument => "-d";
        public int Order => 1;

        public async Task<bool> Execute(ProcessRun run)
        {
            List<string> encryptedFiles = new List<string>();
            Console.WriteLine("Searching for encrypted files");
            encryptedFiles.AddRange(Directory.EnumerateFiles(run.Settings.TempDirectory, $"{Constants.ENCRYPTED_FILE_PREFIX}*", SearchOption.AllDirectories));
            Console.WriteLine($"Found {encryptedFiles.Count} encrypted files.");

            try
            {
                foreach (string file in encryptedFiles)
                {
                    string path = Path.GetDirectoryName(file) ?? "/";
                    string originalFilename = Path.GetFileName(file);

                    if (!originalFilename.StartsWith(Constants.ENCRYPTED_FILE_PREFIX))
                    {
                        Console.WriteLine($"Tried to decrypt file which did not match the prefix. File: {file}. Skipping.");
                        continue;
                    }

                    string decryptedFilepath = Path.Combine(path, originalFilename.Substring(Constants.ENCRYPTED_FILE_PREFIX.Length));

                    Console.WriteLine($"Started decrypting {originalFilename} to {decryptedFilepath}");

                    await CryptoUtils.DecryptFile(file, decryptedFilepath, run.Settings.EncryptionKey, run.Settings.EncryptionIV);

                    Console.WriteLine($"Completed decrypting {originalFilename} to {decryptedFilepath}");

                    Console.WriteLine($"Deleting {originalFilename}");
                    File.Delete(file);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to decrypt file. The process will now stop.\nError:{e}");
                return false;
            }
            
            // Renaming folders
            Console.WriteLine("Renaming folders to use unencrypted naming scheme.");
            foreach (string folderPath in Directory.EnumerateDirectories(run.Settings.TempDirectory, $"{Constants.ENCRYPTED_FILE_PREFIX}*", SearchOption.AllDirectories))
            {
                DirectoryInfo directory = new DirectoryInfo(folderPath);

                string? path = directory.Parent?.FullName ?? "/";

                string originalName = directory.Name;

                if (!originalName.StartsWith(Constants.ENCRYPTED_FILE_PREFIX))
                {
                    Console.WriteLine($"Tried to rename folder which did not match the prefix. Folder: {originalName}. Skipping.");
                    continue;
                }

                string newName = originalName.Substring(Constants.ENCRYPTED_FILE_PREFIX.Length);
                Directory.Move(Path.Combine(path, originalName), Path.Combine(path, newName));
            }

            return true;
        }

        public bool IsCompleted(ProcessRun run)
        {
            return Directory.EnumerateFiles(run.Settings.TempDirectory, $"{Constants.ENCRYPTED_FILE_PREFIX}*", SearchOption.AllDirectories).Count() == 0;
        }
    }
}
