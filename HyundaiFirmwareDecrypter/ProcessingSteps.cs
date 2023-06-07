using HyundaiFirmwareDecrypter.ProcessSteps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyundaiFirmwareDecrypter
{
    internal static class ProcessingSteps
    {
        public static ExtractStep Extract = new ExtractStep();
        public static DecryptStep Decrypt = new DecryptStep();
        public static InstallAdbdBackdoor InstallAdbdBackdoor = new InstallAdbdBackdoor();
        public static CalculateHashesStep CalculateHashes = new CalculateHashesStep();
        public static CreateZipStep CreateZip = new CreateZipStep();

        public static List<IProcessStep> StepLookupByOrder = BuildStepLookupByOrder();
        public static Dictionary<string, IProcessStep> StepLookupByArgument = BuildStepLookupByArgument();

        public static IEnumerable<IProcessStep> GetSteps()
        {
            yield return Extract;
            yield return Decrypt;
            yield return InstallAdbdBackdoor;
            yield return CalculateHashes;
            yield return CreateZip;
        }

        public static Dictionary<string, IProcessStep> BuildStepLookupByArgument()
        {
            return GetSteps().ToDictionary(s => s.Argument, StringComparer.OrdinalIgnoreCase);
        }

        public static List<IProcessStep> BuildStepLookupByOrder()
        {
            return GetSteps().OrderBy(s => s.Order).ToList();
        }

        public static string GetHelp()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Select the arguments followed by the encrypted system update file to work on.");

            sb.AppendLine("Arguments: ");
            foreach (IProcessStep step in GetSteps())
            {
                sb.AppendLine($"{step.Name} ({step.Argument}) - {step.Description}");
            }

            sb.AppendLine($"All (-a) - Extract, Decrypt, Install ADB Backdoor, Calculate Hashes, and Create the Unencrypted Zip file.");
            sb.AppendLine();
            sb.AppendLine($"Example: {AppDomain.CurrentDomain.FriendlyName} -x -d -c enc_d2vsystem_package_134.100.220927.zip");
            sb.AppendLine($"Example: {AppDomain.CurrentDomain.FriendlyName} -a enc_d2vsystem_package_134.100.220927.zip");

            return sb.ToString();
        }
    }
}
