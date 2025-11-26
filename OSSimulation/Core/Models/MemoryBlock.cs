using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace OSSimulation.Core.Models
{
    public class MemoryBlock : INotifyPropertyChanged
    {
        private bool _isAllocated;
        private int _ownerPid;

        public int FrameNumber { get; set; }
        public long Address { get; set; }
        public int Size { get; set; }

        public bool IsAllocated
        {
            get => _isAllocated;
            set
            {
                _isAllocated = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(StatusColor));
            }
        }

        public int OwnerPid
        {
            get => _ownerPid;
            set
            {
                _ownerPid = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(StatusColor));
            }
        }

        public string StatusColor => IsAllocated ? "#4CAF50" : "#424242";

        public MemoryBlock(int frameNumber, long address, int size)
        {
            FrameNumber = frameNumber;
            Address = address;
            Size = size;
            IsAllocated = false;
            OwnerPid = 0;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}