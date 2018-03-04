using BlackjackSimulator.Entities;

namespace BlackjackSimulator.Interfaces
{
    public interface ISimulationScenario
    {
        TableSimulation BuildSimulation();
    }
}