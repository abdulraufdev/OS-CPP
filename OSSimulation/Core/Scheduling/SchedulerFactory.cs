using OSSimulation.Core.Scheduling.Algorithms;

namespace OSSimulation.Core.Scheduling
{
    public static class SchedulerFactory
    {
        public static IScheduler CreateScheduler(SchedulingAlgorithm algorithm)
        {
            return algorithm switch
            {
                SchedulingAlgorithm.FCFS => new FCFSScheduler(),
                SchedulingAlgorithm.SJF => new SJFScheduler(),
                SchedulingAlgorithm.RoundRobin => new RoundRobinScheduler(),
                _ => new FCFSScheduler()
            };
        }
    }
}
