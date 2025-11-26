using System;
using System.Collections.Generic;
using System.Linq;

namespace OSSimulation.Core.Models
{
    public class SystemState
    {
        public List<Process> AllProcesses { get; set; } = new();
        public List<Process> ReadyQueue { get; set; } = new();
        public Process? RunningProcess { get; set; }
        public List<Process> BlockedProcesses { get; set; } = new();
        public List<Process> TerminatedProcesses { get; set; } = new();

        public Dictionary<string, Resource> Resources { get; set; } = new();

        public int TotalMemoryMB { get; set; } = 1024;
        public int UsedMemoryMB { get; set; }
        public int AvailableMemoryMB => TotalMemoryMB - UsedMemoryMB;

        public DateTime SimulationStartTime { get; set; }
        public TimeSpan ElapsedTime => DateTime.Now - SimulationStartTime;

        public int ContextSwitches { get; set; }
        public int DeadlocksDetected { get; set; }
        public int DeadlocksResolved { get; set; }

        public double CpuUtilization { get; set; }
        public double Throughput { get; set; }

        public SystemState()
        {
            SimulationStartTime = DateTime.Now;
        }

        public double GetAverageWaitingTime()
        {
            if (AllProcesses.Count == 0) return 0;
            return AllProcesses.Average(p => p.WaitingTime.TotalMilliseconds);
        }

        public double GetAverageTurnaroundTime()
        {
            var completed = TerminatedProcesses.Where(p => p.TurnaroundTime > TimeSpan.Zero).ToList();
            if (completed.Count == 0) return 0;
            return completed.Average(p => p.TurnaroundTime.TotalMilliseconds);
        }
    }
}