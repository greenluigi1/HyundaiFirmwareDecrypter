using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace HyundaiFirmwareDecrypter
{
    internal static class FirmwareUtils
    {
        public static async Task<bool> MountImage(string pathToImage, string pathToMountFolder)
        {
            ProcessStartInfo mountStartInfo = new ProcessStartInfo("mount");
            mountStartInfo.ArgumentList.Add("-o");
            mountStartInfo.ArgumentList.Add("loop,rw,sync");
            mountStartInfo.ArgumentList.Add(pathToImage);
            mountStartInfo.ArgumentList.Add(pathToMountFolder);

            Process? mount = Process.Start(mountStartInfo);

            if (mount == null)
            {
                return false;
            }

            await mount.WaitForExitAsync();

            return mount.ExitCode == 0;
        }

        public static async Task<bool> Unmount(string pathToMountFolder)
        {
            ProcessStartInfo unmountStartInfo = new ProcessStartInfo("umount");
            unmountStartInfo.ArgumentList.Add(pathToMountFolder);

            Process? unmount = Process.Start(unmountStartInfo);

            if (unmount == null)
            {
                return false;
            }

            await unmount.WaitForExitAsync();

            return unmount.ExitCode == 0;
        }

        public static async Task<bool> ChmodFile(string pathToChmod, string chmodValue)
        {
            ProcessStartInfo chmodStartInfo = new ProcessStartInfo("chmod");
            chmodStartInfo.ArgumentList.Add(chmodValue);
            chmodStartInfo.ArgumentList.Add(pathToChmod);

            Process? chmodCommand = Process.Start(chmodStartInfo);

            if (chmodCommand == null)
            {
                return false;
            }

            await chmodCommand.WaitForExitAsync();

            return chmodCommand.ExitCode == 0;
        }
        public static async Task<bool> CreateSymlink(string sourcePath, string symlinkPath)
        {
            ProcessStartInfo lnStartInfo = new ProcessStartInfo("ln");
            lnStartInfo.ArgumentList.Add("-s");
            lnStartInfo.ArgumentList.Add("-f");
            lnStartInfo.ArgumentList.Add(sourcePath);
            lnStartInfo.ArgumentList.Add(symlinkPath);

            Process? lnCommand = Process.Start(lnStartInfo);

            if (lnCommand == null)
            {
                return false;
            }

            await lnCommand.WaitForExitAsync();

            return lnCommand.ExitCode == 0;
        }
    }
}
