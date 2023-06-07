using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyundaiFirmwareDecrypter
{
    internal record ProcessRun(string EncryptedUpdateLocation, FirmwareSettings Settings);
}
