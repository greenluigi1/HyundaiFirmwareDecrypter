using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyundaiFirmwareDecrypter.ProcessSteps
{
    internal class CreateMasterFileListStep : IProcessStep
    {
        public string Name => "Create Master File List";

        public string Description => "Create Master File List.";

        public string Argument => "-t";
        public int Order => 0;

        public IEnumerable<IProcessStep> GetDependents()
        {
            return Enumerable.Empty<IProcessStep>();
        }

        public async Task<bool> Execute(ProcessRun run)
        {
            string encryptedUpdateHashListPath = Path.Combine(run.Settings.TempDirectory, Constants.FILE_HASH_LIST_FILENAME_FOR_ENCRYPTED_UPDATE);
            string unencryptedUpdateHashListPath = Path.Combine(run.Settings.TempDirectory, Constants.FILE_HASH_LIST_FILENAME_FOR_UNENCRYPTED_UPDATE);

            List<string> previousFileHashes = (await File.ReadAllLinesAsync(encryptedUpdateHashListPath)).ToList();
            HashSet<string> fileList = new HashSet<string>(previousFileHashes.Select(e => e.Split(':')[0]).ToHashSet(StringComparer.OrdinalIgnoreCase));

            // Check to see which files don't have hashes for them
            foreach (string file in Directory.EnumerateFiles(run.Settings.TempDirectory, "*", SearchOption.AllDirectories))
            {
                string relativePath = Path.GetRelativePath(run.Settings.TempDirectory, file);
                relativePath = relativePath.Replace("\\", "/");

                if (!fileList.Contains(relativePath))
                {
                    Console.WriteLine($"Difference Found: {relativePath} not in hash list.");
                }
                else
                {
                    Console.WriteLine($"Matched: {relativePath}");
                }
            }

            return true;
        }

        public bool IsCompleted(ProcessRun run) => false;
    }
}
