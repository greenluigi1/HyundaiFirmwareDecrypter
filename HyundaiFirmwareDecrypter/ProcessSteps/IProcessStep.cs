using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyundaiFirmwareDecrypter.ProcessSteps
{
    internal interface IProcessStep
    {
        string Name { get; }
        string Description { get; }
        string Argument { get; }
        int Order { get; }

        bool IsCompleted(ProcessRun run);
        Task<bool> Execute(ProcessRun run);
    }
}
