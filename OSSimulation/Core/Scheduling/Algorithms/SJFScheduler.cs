using System.Collections.Generic;
using System.Linq;
using OSSimulation.Core.Models;

namespace OSSimulation.Core.Scheduling.Algorithms
{
    public class SJFScheduler : IScheduler
    {
        public Process? GetNextProcess(List<Process> readyQueue, Process? currentRunning)
        {
            if (currentRunning != null && currentRunning.RemainingBurstTime > 0)
                return currentRunning;
            
            return readyQueue
                .Where(p => p.State == ProcessState.Ready)
                .OrderBy(p => p.BurstTime)
                .ThenBy(p => p.ArrivalTime)
                .FirstOrDefault();
        }
        
        public string GetAlgorithmName() => "SJF";
        public void OnTick(int elapsedMs) { }
        public void Reset() { }
    }
}
