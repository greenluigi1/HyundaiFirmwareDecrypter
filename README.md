# Hyundai Firmware Decrypter
Hyundai Firmware Decrypter is a linux-only tool used to decrypt and modify D-Audio2 firmware updates.

"./HyundaiFirmwareDecrypter" will need to be run once to generate the FirmwareSettings.json settings file.

Usage:
```
Arguments:
Extract (-x) - Extract the specified encrypted system update file
Decrypt (-d) - Decrypt all of the extracted files.
Install ADB Backdoor (-i) - Enables the Android Debug Bridge TCP server which can be used to access a root shell. Needs to be run as root/with sudo.
Calculate Hashes (-c) - Calculate and compile the list of the SHA512 hashes for all of the update files.
Create Zip (-z) - Create the unencrypted system update zip file.
All (-a) - Extract, Decrypt, Install ADB Backdoor, Calculate Hashes, and Create the Unencrypted Zip file.

Example: HyundaiFirmwareDecrypter -x -d -c enc_d2vsystem_package_134.100.220927.zip
Example: HyundaiFirmwareDecrypter -a enc_d2vsystem_package_134.100.220927.zip
```