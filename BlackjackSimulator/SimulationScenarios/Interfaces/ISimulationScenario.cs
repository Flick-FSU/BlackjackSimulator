using BlackjackSimulator.Entities;
using BlackjackSimulator.Entities.Interfaces;
using BlackjackSimulator.Models;

namespace BlackjackSimulator.SimulationScenarios.Interfaces
{
    public interface ISimulationScenario
    {
        SimulationProperties GetSimulationProperties();
    }
}