using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace OSSimulation.Core.Models
{
    /// <summary>
    /// Represents a process in the OS simulation.
    /// Implements the Process Control Block (PCB) concept from OS theory.
    /// </summary>
    /// <remarks>
    /// Each process tracks its state, resource allocation, and execution metrics.
    /// Reference: Silberschatz, Galvin, Gagne - Operating System Concepts, Ch. 3
    /// </remarks>
    public class Process : INotifyPropertyChanged
    {
        private static int _nextPid = 1;
        private ProcessState _state;
        private int _remainingBurstTime;
        private DateTime? _startTime;
        private DateTime? _completionTime;
        private string _currentScheduler = "";

        public int PID { get; }
        public string Name { get; set; }

        /// <summary>
        /// Total CPU time required (in milliseconds). 
        /// </summary>
        public int BurstTime { get; set; }

        /// <summary>
        /// Remaining CPU time (decrements during execution).
        /// Time Complexity: O(1) for update
        /// </summary>
        public int RemainingBurstTime
        {
            get => _remainingBurstTime;
            set
            {
                _remainingBurstTime = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Progress));
            }
        }

        /// <summary>
        /// Execution progress (0. 0 to 1.0) for UI visualization.
        /// </summary>
        public double Progress => BurstTime > 0 ? 1.0 - ((double)RemainingBurstTime / BurstTime) : 1.0;

        /// <summary>
        /// Process priority (1-10, higher = more important).
        /// Used by Priority Scheduling algorithm.
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// Memory required (in MB).
        /// Allocated via paging in MemoryManager.
        /// </summary>
        public int MemoryRequired { get; set; }

        /// <summary>
        /// Arrival time in the system. 
        /// </summary>
        public DateTime ArrivalTime { get; set; }

        /// <summary>
        /// Current process state.
        /// </summary>
        public ProcessState State
        {
            get => _state;
            set
            {
                var oldState = _state;
                _state = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(StateColor));
                OnPropertyChanged(nameof(StateText));

                // Track first time running for response time calculation
                if (value == ProcessState.Running && _startTime == null)
                    _startTime = DateTime.Now;

                if (value == ProcessState.Terminated)
                    _completionTime = DateTime.Now;
            }
        }

        /// <summary>
        /// Color for UI binding based on state.
        /// Maps to dark theme color scheme.
        /// </summary>
        public string StateColor => State switch
        {
            ProcessState.New => "#2196F3",      // Blue
            ProcessState.Ready => "#FFC107",    // Amber
            ProcessState.Running => "#4CAF50",  // Green
            ProcessState.Blocked => "#FF5722",  // Deep Orange
            ProcessState.Terminated => "#9E9E9E", // Gray
            _ => "#FFFFFF"
        };

        /// <summary>
        /// Human-readable state text.
        /// </summary>
        public string StateText => State.ToString();

        /// <summary>
        /// Resources currently requested by this process.
        /// </summary>
        public HashSet<string> RequestedResources { get; set; } = new();

        /// <summary>
        /// Resources currently allocated to this process.
        /// </summary>
        public HashSet<string> AllocatedResources { get; set; } = new();

        /// <summary>
        /// Time spent waiting in ready queue.
        /// </summary>
        public TimeSpan WaitingTime { get; set; }

        /// <summary>
        /// Turnaround time = Completion Time - Arrival Time. 
        /// Key performance metric for scheduling algorithms.
        /// </summary>
        public TimeSpan TurnaroundTime
        {
            get
            {
                if (_completionTime.HasValue)
                    return _completionTime.Value - ArrivalTime;
                return TimeSpan.Zero;
            }
        }

        /// <summary>
        /// Response time = First time process gets CPU - Arrival Time.
        /// Important for interactive systems.
        /// </summary>
        public TimeSpan ResponseTime
        {
            get
            {
                if (_startTime.HasValue)
                    return _startTime.Value - ArrivalTime;
                return TimeSpan.Zero;
            }
        }

        /// <summary>
        /// Current scheduler managing this process (for logging).
        /// </summary>
        public string CurrentScheduler
        {
            get => _currentScheduler;
            set
            {
                _currentScheduler = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Time quantum remaining (for Round Robin).
        /// </summary>
        public int QuantumRemaining { get; set; }

        /// <summary>
        /// Number of pages allocated to this process.
        /// </summary>
        public int PageCount { get; set; }

        public Process()
        {
            PID = _nextPid++;
            Name = $"P{PID}";
            State = ProcessState.New;
            ArrivalTime = DateTime.Now;
        }

        public Process(string name, int burstTime, int priority, int memoryMB, HashSet<string> resources)
        {
            PID = _nextPid++;
            Name = name;
            BurstTime = burstTime;
            RemainingBurstTime = burstTime;
            Priority = priority;
            MemoryRequired = memoryMB;
            RequestedResources = resources;
            State = ProcessState.New;
            ArrivalTime = DateTime.Now;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString() => $"P{PID} ({Name})";
    }
}