namespace OSSimulation.Core.Models
{
    /// <summary>
    /// Process states based on OS process lifecycle. 
    /// </summary>
    public enum ProcessState
    {
        New,        // Just created
        Ready,      // Ready to run, waiting for CPU
        Running,    // Currently executing
        Blocked,    // Waiting for resource/I/O
        Terminated  // Execution complete
    }
}