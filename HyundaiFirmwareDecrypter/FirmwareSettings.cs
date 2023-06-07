using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HyundaiFirmwareDecrypter
{
    public class FirmwareSettings
    {
        public string TempDirectory { get; set; } = "extract_temp";
        public string SystemImageMountDirectory { get; set; } = "system_image";
        public string ZipPassword { get; set; } = "$09#$모비스98@!OTA$$";
        public string EncryptionKeyHex { get; set; } = "F4DA05A5E848309EE8377464CF4254A3";
        public string EncryptionIVHex { get; set; } = "763AC2AFAC9E8EE379788B6CC08612B0";


        private byte[]? encryptionKey;
        [JsonIgnore]
        public byte[] EncryptionKey
        {
            get
            {
                if (encryptionKey == null)
                {
                    encryptionKey = Convert.FromHexString(EncryptionKeyHex);
                }

                return encryptionKey;
            }
        }

        private byte[]? encryptionIV;

        [JsonIgnore]
        public byte[] EncryptionIV
        {
            get
            {
                if (encryptionIV == null)
                {
                    encryptionIV = Convert.FromHexString(EncryptionIVHex);
                }

                return encryptionIV;
            }
        }
    }
}
