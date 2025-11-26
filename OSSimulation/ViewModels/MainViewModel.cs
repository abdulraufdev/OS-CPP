using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Threading;
using OSSimulation.Core.Models;

namespace OSSimulation.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly DispatcherTimer _simulationTimer;
        private readonly SystemState _systemState;
        private bool _isRunning;
        private string _statusText;
        private string _simulationTime;

        // Observable collections for UI binding
        public ObservableCollection<Process> ProcessList { get; set; }
        public ObservableCollection<string> ActivityLog { get; set; }

        // Metrics
        private string _cpuUtilization = "0%";
        private string _avgWaitTime = "0 ms";
        private string _throughput = "0/s";
        private string _totalProcesses = "0";

        public string CpuUtilization
        {
            get => _cpuUtilization;
            set { _cpuUtilization = value; OnPropertyChanged(); }
        }

        public string AvgWaitTime
        {
            get => _avgWaitTime;
            set { _avgWaitTime = value; OnPropertyChanged(); }
        }

        public string Throughput
        {
            get => _throughput;
            set { _throughput = value; OnPropertyChanged(); }
        }

        public string TotalProcesses
        {
            get => _totalProcesses;
            set { _totalProcesses = value; OnPropertyChanged(); }
        }

        public string StatusText
        {
            get => _statusText;
            set { _statusText = value; OnPropertyChanged(); }
        }

        public string SimulationTime
        {
            get => _simulationTime;
            set { _simulationTime = value; OnPropertyChanged(); }
        }

        public bool IsRunning
        {
            get => _isRunning;
            set { _isRunning = value; OnPropertyChanged(); }
        }

        public MainViewModel()
        {
            _systemState = new SystemState();
            ProcessList = new ObservableCollection<Process>();
            ActivityLog = new ObservableCollection<string>();

            // Initialize timer (10ms tick = 100 FPS simulation)
            _simulationTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(10)
            };
            _simulationTimer.Tick += SimulationTimer_Tick;

            StatusText = "Ready to simulate";
            SimulationTime = "00:00:00";

            AddLog("System initialized");
            AddLog("Ready to create processes");
        }

        // Create single process
        public void CreateProcess(string name, int burstTime, int priority, int memoryMB)
        {
            try
            {
                var process = new Process(name, burstTime, priority, memoryMB, new System.Collections.Generic.HashSet<string>());

                _systemState.AllProcesses.Add(process);
                _systemState.ReadyQueue.Add(process);
                process.State = ProcessState.Ready;

                ProcessList.Add(process);

                UpdateMetrics();
                AddLog($"✅ Created: {process.Name} (Burst: {burstTime}ms, Priority: {priority})");
            }
            catch (Exception ex)
            {
                AddLog($"❌ Error creating process: {ex.Message}");
            }
        }

        // Auto-generate 10 random processes
        public void AutoGenerateProcesses()
        {
            string[] names = { "Chrome", "VSCode", "Spotify", "Discord", "Slack",
                             "Teams", "Excel", "Photoshop", "Steam", "Zoom",
                             "Firefox", "Notepad", "Calculator", "Paint", "OneDrive" };

            Random rand = new Random();

            for (int i = 0; i < 10; i++)
            {
                string name = names[rand.Next(names.Length)] + $" {i + 1}";
                int burstTime = rand.Next(100, 1000);  // 100-1000ms
                int priority = rand.Next(1, 11);        // 1-10
                int memory = rand.Next(50, 500);        // 50-500 MB

                CreateProcess(name, burstTime, priority, memory);
            }

            AddLog($"🎲 Auto-generated 10 random processes");
        }

        // Start simulation
        public void StartSimulation()
        {
            if (ProcessList.Count == 0)
            {
                AddLog("⚠️ No processes to simulate!  Create some first.");
                return;
            }

            IsRunning = true;
            _systemState.SimulationStartTime = DateTime.Now;
            _simulationTimer.Start();

            StatusText = "Simulation running... ";
            AddLog("▶️ Simulation started");
        }

        // Pause simulation
        public void PauseSimulation()
        {
            IsRunning = false;
            _simulationTimer.Stop();

            StatusText = "Simulation paused";
            AddLog("⏸️ Simulation paused");
        }

        // Main simulation tick
        private void SimulationTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                // Update simulation time
                SimulationTime = _systemState.ElapsedTime.ToString(@"hh\:mm\:ss");

                // Simple FCFS scheduling for now
                if (_systemState.RunningProcess == null && _systemState.ReadyQueue.Count > 0)
                {
                    // Get next process
                    var nextProcess = _systemState.ReadyQueue.OrderBy(p => p.ArrivalTime).First();
                    _systemState.ReadyQueue.Remove(nextProcess);

                    nextProcess.State = ProcessState.Running;
                    _systemState.RunningProcess = nextProcess;

                    AddLog($"▶️ Running: {nextProcess.Name}");
                }

                // Execute current process
                if (_systemState.RunningProcess != null)
                {
                    var process = _systemState.RunningProcess;

                    // Decrement remaining time (10ms per tick)
                    process.RemainingBurstTime = Math.Max(0, process.RemainingBurstTime - 10);

                    // Check if completed
                    if (process.RemainingBurstTime == 0)
                    {
                        process.State = ProcessState.Terminated;
                        _systemState.TerminatedProcesses.Add(process);
                        _systemState.RunningProcess = null;

                        AddLog($"✅ Completed: {process.Name} (Turnaround: {process.TurnaroundTime.TotalMilliseconds:F0}ms)");
                    }
                }

                // Update metrics
                UpdateMetrics();

                // Check if simulation is done
                if (_systemState.ReadyQueue.Count == 0 && _systemState.RunningProcess == null)
                {
                    PauseSimulation();
                    StatusText = "All processes completed! ";
                    AddLog("🎉 All processes completed!");
                }
            }
            catch (Exception ex)
            {
                AddLog($"❌ Simulation error: {ex.Message}");
                PauseSimulation();
            }
        }

        // Update metrics
        private void UpdateMetrics()
        {
            // CPU Utilization
            double cpuUtil = _systemState.RunningProcess != null ? 100.0 : 0.0;
            CpuUtilization = $"{cpuUtil:F0}%";

            // Average Wait Time
            double avgWait = _systemState.GetAverageWaitingTime();
            AvgWaitTime = $"{avgWait:F0} ms";

            // Throughput (processes per second)
            if (_systemState.ElapsedTime.TotalSeconds > 0)
            {
                double throughput = _systemState.TerminatedProcesses.Count / _systemState.ElapsedTime.TotalSeconds;
                Throughput = $"{throughput:F2}/s";
            }

            // Total Processes
            TotalProcesses = _systemState.AllProcesses.Count.ToString();
        }

        // Add log entry
        private void AddLog(string message)
        {
            string timestamp = DateTime.Now.ToString("HH:mm:ss");
            ActivityLog.Insert(0, $"[{timestamp}] {message}");

            // Keep only last 100 entries
            while (ActivityLog.Count > 100)
            {
                ActivityLog.RemoveAt(ActivityLog.Count - 1);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}