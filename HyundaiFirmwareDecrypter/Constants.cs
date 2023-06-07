using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyundaiFirmwareDecrypter
{
    internal class Constants
    {
        public const string SETTINGS_FILENAME = "FirmwareSettings.json";
        
        public const string ENCRYPTED_FILE_PREFIX = "enc_";
        public const string SIGNATURE_FILENAME_FOR_ENCRYPTED_UPDATE = "update.info";
        public const string FILE_HASH_LIST_FILENAME_FOR_ENCRYPTED_UPDATE = "update.cfg";
        public const string FILE_HASH_LIST_FILENAME_FOR_UNENCRYPTED_UPDATE = "update.list";

        public const string UNENCRYPTED_UPDATE_FILENAME = "system_update.zip";

        public const int SHA_BUFFER_SIZE = 16777088;

        public const string ADBD_BACKDOOR_SERVICE = @"[Unit]
Description=Android Debug Bridge TCP

[Service]
Type=forking
Restart=on-failure
ExecStart=/usr/sbin/start-stop-daemon -S --pidfile /var/run/adbdtcp.pid --make-pidfile --background -x /usr/bin/adbd tcpip
ExecStop=/usr/sbin/start-stop-daemon -K -p /var/run/adbdtcp.pid -o -x /usr/bin/adbd
ExecStopPost=/bin/rm -f /var/run/adbdtcp.pid
StandardOutput=null

[Install]
WantedBy=basic.target
";
    }
}
