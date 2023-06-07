using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyundaiFirmwareDecrypter.ProcessSteps
{
    internal class ExtractStep : IProcessStep
    {
        public string Name => "Extract";

        public string Description => "Extract the specified encrypted system update file";

        public string Argument => "-x";
        public int Order => 0;

        public Task<bool> Execute(ProcessRun run)
        {
            if (!Directory.Exists(run.Settings.TempDirectory))
            {
                Console.WriteLine($"Creating temp directory: {run.Settings.TempDirectory}");
                Directory.CreateDirectory(run.Settings.TempDirectory);
            }
            else if (Directory.EnumerateFileSystemEntries(run.Settings.TempDirectory).Any())
            {
                if (!ConsoleHelper.PromptForConfirmation($"Temp directory {run.Settings.TempDirectory} appears to have files in it. Clear the temp directory?"))
                {
                    Console.WriteLine("Clear out the temp directory to continue or remove the Extract step argument.");
                    return Task.FromResult(false);
                }
                else
                {
                    FileUtils.ClearDirectory(run.Settings.TempDirectory);
                }
            }

            Console.WriteLine("Extracting update to temp directory");

            try
            {
                FastZip zip = new FastZip();
                zip.Password = run.Settings.ZipPassword;
                zip.ExtractZip(run.EncryptedUpdateLocation, run.Settings.TempDirectory, null!);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to extract update.\nError: {e}");
                return Task.FromResult(false);
            }

            Console.WriteLine("Completed update extraction");
            return Task.FromResult(true);
        }

        public bool IsCompleted(ProcessRun run)
        {
            return Directory.Exists(run.Settings.TempDirectory) && Directory.EnumerateFileSystemEntries(run.Settings.TempDirectory).Any();
        }
    }
}
