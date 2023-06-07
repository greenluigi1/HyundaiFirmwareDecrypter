using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyundaiFirmwareDecrypter
{
    internal static class FileUtils
    {
        public static void ClearDirectory(string path)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(path);

            foreach (FileInfo fileInfo in directoryInfo.EnumerateFiles())
            {
                fileInfo.Delete();
            }

            foreach (DirectoryInfo directory in directoryInfo.EnumerateDirectories())
            {
                directory.Delete(true);
            }
        }
    }
}
