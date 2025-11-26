using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace OSSimulation.Core.Models
{
    public class Resource : INotifyPropertyChanged
    {
        private int _available;
        private int _allocated;

        public string Name { get; set; }
        public int TotalInstances { get; set; }

        public int Available
        {
            get => _available;
            set
            {
                _available = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Utilization));
            }
        }

        public int Allocated
        {
            get => _allocated;
            set
            {
                _allocated = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Utilization));
            }
        }

        public double Utilization => TotalInstances > 0 ? (double)Allocated / TotalInstances : 0;
        public List<int> HoldingProcesses { get; set; } = new();
        public Queue<int> WaitingProcesses { get; set; } = new();

        public Resource(string name, int totalInstances)
        {
            Name = name;
            TotalInstances = totalInstances;
            Available = totalInstances;
            Allocated = 0;
        }

        public bool Allocate(int processId)
        {
            if (Available > 0)
            {
                Available--;
                Allocated++;
                HoldingProcesses.Add(processId);
                return true;
            }
            return false;
        }

        public void Release(int processId)
        {
            if (HoldingProcesses.Remove(processId))
            {
                Available++;
                Allocated--;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString() => $"{Name} ({Available}/{TotalInstances} available)";
    }
}