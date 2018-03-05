using BlackjackSimulator.Entities;

namespace BlackjackSimulator.SimulationScenarios.Interfaces
{
    public interface ISimulationScenario
    {
        TableSimulation BuildSimulation();
    }
}