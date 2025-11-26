using System.Collections.Generic;
using System.Linq;
using OSSimulation.Core.Models;

namespace OSSimulation.Core.Scheduling.Algorithms
{
    public class RoundRobinScheduler : IScheduler
    {
        private int _timeQuantum = 50;
        private int _currentQuantumUsed = 0;
        private Process? _lastProcess;
        
        public Process? GetNextProcess(List<Process> readyQueue, Process? currentRunning)
        {
            var ready = readyQueue.Where(p => p.State == ProcessState.Ready).ToList();
            
            if (ready.Count == 0)
                return null;
            
            if (currentRunning != null && currentRunning.RemainingBurstTime > 0)
            {
                if (_currentQuantumUsed < _timeQuantum)
                    return currentRunning;
                else
                    _currentQuantumUsed = 0;
            }
            
            var next = ready.First();
            if (next != _lastProcess)
            {
                _currentQuantumUsed = 0;
                _lastProcess = next;
            }
            
            return next;
        }
        
        public void OnTick(int elapsedMs)
        {
            _currentQuantumUsed += elapsedMs;
        }
        
        public string GetAlgorithmName() => "Round Robin (q=50ms)";
        
        public void Reset()
        {
            _currentQuantumUsed = 0;
            _lastProcess = null;
        }
    }
}
