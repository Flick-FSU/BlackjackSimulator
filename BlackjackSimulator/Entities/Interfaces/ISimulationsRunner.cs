using BlackjackSimulator.Models;

namespace BlackjackSimulator.Entities.Interfaces
{
    public interface ISimulationsRunner
    {
        void Load(SimulationProperties simulationProperties);
        void Run(int numberOfSimulationRuns);
    }
}