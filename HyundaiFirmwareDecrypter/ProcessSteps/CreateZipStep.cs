using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyundaiFirmwareDecrypter.ProcessSteps
{
    internal class CreateZipStep : IProcessStep
    {
        public string Name => "Create Zip";

        public string Description => "Create the unencrypted system update zip file.";

        public string Argument => "-z";
        public int Order => 4;

        public async Task<bool> Execute(ProcessRun run)
        {
            Console.WriteLine($"Creating unencrypted update zip");
            if (File.Exists(Constants.UNENCRYPTED_UPDATE_FILENAME) && ConsoleHelper.PromptForConfirmation($"{Constants.UNENCRYPTED_UPDATE_FILENAME} already exists. Delete?"))
            {
                File.Delete(Constants.UNENCRYPTED_UPDATE_FILENAME);
            }

            try
            {
                string outputZipFullPath = Path.GetFullPath(Constants.UNENCRYPTED_UPDATE_FILENAME);
                string tempFullPath = Path.GetFullPath(run.Settings.TempDirectory);
                Console.WriteLine("Running zip command (This may take a minute)");
                ProcessStartInfo zipStartInfo = new ProcessStartInfo("zip", $"-r {outputZipFullPath} ./");
                zipStartInfo.WorkingDirectory = tempFullPath;

                Console.WriteLine($"Running command: {zipStartInfo.FileName} {zipStartInfo.Arguments} (in:{zipStartInfo.WorkingDirectory})");

                Process? zipProcess = Process.Start(zipStartInfo);

                if (zipProcess == null)
                {
                    Console.WriteLine("Failed to start zip");
                    return false;
                }

                await zipProcess.WaitForExitAsync();

                if (zipProcess.ExitCode != 0)
                {
                    Console.WriteLine("Extract Failed");
                    return false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error encountered when creating update zip.\nError: {e}");
                return false;
            }

            Console.WriteLine($"Successfully created unencrypted system update zip: {Constants.UNENCRYPTED_UPDATE_FILENAME}");

            if (ConsoleHelper.PromptForConfirmation($"Clear temp directory at {run.Settings.TempDirectory}?"))
            {
                FileUtils.ClearDirectory(run.Settings.TempDirectory);
            }

            return true;
        }

        public bool IsCompleted(ProcessRun run)
        {
            return File.Exists(Constants.UNENCRYPTED_UPDATE_FILENAME);
        }
    }
}
