using HyundaiFirmwareDecrypter.ProcessSteps;
using ICSharpCode.SharpZipLib.Zip;
using Org.BouncyCastle.Crypto.Digests;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading;

namespace HyundaiFirmwareDecrypter
{
    internal class Program
    {
        private static FirmwareSettings settings = null!;

        static async Task Main(string[] args)
        {
            if (!File.Exists(Constants.SETTINGS_FILENAME))
            {
                Console.WriteLine($"Fill out {Constants.SETTINGS_FILENAME} with the appropriate settings");
                string defaultSettingsJson = JsonSerializer.Serialize(new FirmwareSettings(), new JsonSerializerOptions()
                {
                    WriteIndented = true,
                });

                File.WriteAllText(Constants.SETTINGS_FILENAME, defaultSettingsJson);
                return;
            }

            try
            {
                string settingsJson = File.ReadAllText(Constants.SETTINGS_FILENAME);
                settings = JsonSerializer.Deserialize<FirmwareSettings>(settingsJson)!;

                if (settings == null)
                {
                    Console.WriteLine($"Failed to read settings file from {Constants.SETTINGS_FILENAME}. Fix the file or remove it and restart the application to regenerate the file.");
                    return;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to read settings from {Constants.SETTINGS_FILENAME}. \nError: {e}");
                return;
            }

            HashSet<IProcessStep> stepsToRun = new HashSet<IProcessStep>();
            string? systemUpdateZip = null;

            foreach (string argument in args)
            {
                if (ProcessingSteps.StepLookupByArgument.TryGetValue(argument, out IProcessStep? step))
                {
                    stepsToRun.Add(step);
                }
                else if ("-a".Equals(argument, StringComparison.OrdinalIgnoreCase))
                {
                    foreach (IProcessStep s in ProcessingSteps.GetSteps())
                    {
                        stepsToRun.Add(s);
                    }
                }
                else
                {
                    if (File.Exists(argument))
                    {
                        systemUpdateZip = argument;
                    }
                    else
                    {
                        Console.WriteLine($"Unknown argument: {argument}");
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(systemUpdateZip))
            {
                Console.WriteLine("Missing System Update Zip Argument");
                Console.WriteLine(ProcessingSteps.GetHelp());
                return;
            }

            if (stepsToRun.Count == 0)
            {
                Console.WriteLine("Select at least one step to run.");
                Console.WriteLine(ProcessingSteps.GetHelp());
                return;
            }

            ProcessRun processRun = new ProcessRun(systemUpdateZip, settings);

            List<IProcessStep> executionOrder = stepsToRun.OrderBy(s => s.Order).ToList();

            //TODO: Verify that the previous steps are "complete" before running some in the middle?

            HashSet<IProcessStep> completedSteps = new HashSet<IProcessStep>();

            foreach (IProcessStep step in executionOrder)
            {
                //TODO: Get dependents and check/add them to completed steps?
                bool success = false;
                try
                {
                    success = await step.Execute(processRun);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"An error occured while running the step: {step.Name}.\nError: {e}");
                    return;
                }

                if (!success)
                {
                    Console.WriteLine($"Step: {step.Name} failed. Stopping Processing.");
                    return;
                }
            }

            Console.WriteLine("Completed Process");
        }
    }
}