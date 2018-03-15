using BlackjackSimulator.Models;

namespace BlackjackSimulator.Entities.Interfaces
{
    public interface ITableSimulationFactory
    {
        ITableSimulation CreateTableSimulationFrom(SimulationProperties simulationProperties);
    }
}