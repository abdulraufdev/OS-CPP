using System.Collections.Generic;
using OSSimulation.Core.Models;

namespace OSSimulation.Core.Scheduling
{
    public interface IScheduler
    {
        Process? GetNextProcess(List<Process> readyQueue, Process? currentRunning);
        string GetAlgorithmName();
        void OnTick(int elapsedMs);
        void Reset();
    }
}
