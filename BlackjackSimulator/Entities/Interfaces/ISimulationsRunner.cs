namespace BlackjackSimulator.Entities.Interfaces
{
    public interface ISimulationsRunner
    {
        void Load(ITableSimulation tableSimulation);
        void Run(int numberOfSimulationRuns);
    }
}