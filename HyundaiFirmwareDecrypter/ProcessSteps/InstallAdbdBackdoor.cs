using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyundaiFirmwareDecrypter.ProcessSteps
{
    internal class InstallAdbdBackdoor : IProcessStep
    {
        public string Name => "Install ADB Backdoor";

        public string Description => "Enables the Android Debug Bridge TCP server which can be used to access a root shell. Needs to be run as root/with sudo.";

        public string Argument => "-i";
        public int Order => 3;

        public async Task<bool> Execute(ProcessRun run)
        {
            if (!Directory.Exists(run.Settings.SystemImageMountDirectory))
            {
                Console.WriteLine($"Creating system image mount directory: {run.Settings.SystemImageMountDirectory}");
                Directory.CreateDirectory(run.Settings.SystemImageMountDirectory);
            }
            else if (Directory.EnumerateFileSystemEntries(run.Settings.SystemImageMountDirectory).Any())
            {
                if (
                    !(ConsoleHelper.PromptForConfirmation($"System image mount directory {run.Settings.SystemImageMountDirectory} appears to have files in it. Remount the system image?") &&
                    await FirmwareUtils.Unmount(run.Settings.SystemImageMountDirectory))
                    )
                {
                    Console.WriteLine("Manually unmount the system image to continue or remove the Install ADB Backdoor step argument.");
                    return false;
                }
            }

            Console.WriteLine("Installing ADB TCP Backdoor");

            try
            {
                string pathToSystemImage = Path.Combine(run.Settings.TempDirectory, "system/system.img");

                if (!File.Exists(pathToSystemImage))
                {
                    Console.WriteLine($"Could not find system.img file at: {pathToSystemImage}.\nVerify the update zip is extracted and decrypted before trying to install the ADB backdoor.");
                    return false;
                }

                if (!await FirmwareUtils.MountImage(pathToSystemImage, run.Settings.SystemImageMountDirectory))
                {
                    Console.WriteLine("Failed to mount system image.\nCheck errors and retry.");
                    return false;
                }

                string adbdtcpServicePath = Path.Combine(run.Settings.SystemImageMountDirectory, "lib/systemd/system/adbdtcp.service");

                if (File.Exists(adbdtcpServicePath))
                {
                    Console.WriteLine($"ADBD TCP Service already exists: {adbdtcpServicePath}.");
                    return false;
                }

                Console.WriteLine("Writing ADB TCP Service File");
                await File.WriteAllTextAsync(adbdtcpServicePath, Constants.ADBD_BACKDOOR_SERVICE);

                Console.WriteLine("Setting ADB TCP Service File Permissions");
                if (!await FirmwareUtils.ChmodFile(adbdtcpServicePath, "0644"))
                {
                    Console.WriteLine($"Failed to chmod 0644 {adbdtcpServicePath}");
                    return false;
                }

                Console.WriteLine("Enabling ADB TCP Service");
                string enableAdbdServicePath = Path.Combine(run.Settings.SystemImageMountDirectory, "etc/systemd/system/basic.target.wants/adbdtcp.service");
                if (!await FirmwareUtils.CreateSymlink(adbdtcpServicePath, enableAdbdServicePath))
                {
                    Console.WriteLine($"Failed to enable ADB TCP Service");
                    return false;
                }

                Console.WriteLine("Unmounting system.img");
                if (!await FirmwareUtils.Unmount(run.Settings.SystemImageMountDirectory))
                {
                    Console.WriteLine($"Failed to unmount system.img. Unmount the image before creating update file.");
                    return false;
                }

            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("Installing the ADB Backdoor requires root. Rerun the command with sudo.");
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to install ADB TCP Backdoor.\nError: {e}");
                return false;
            }

            Console.WriteLine("Completed installing ADB TCP backdoor");
            return true;
        }

        public bool IsCompleted(ProcessRun run)
        {
            return false;
        }
    }
}
