using BlackjackSimulator.Models;

namespace BlackjackSimulator.Entities.Interfaces
{
    public interface ISimulationsOutputHandler
    {
        void Print(PlayerSimulationsStatistics simulationsStatistics);
        void OutputSingleSimulationResult(int runIndex, IPlayer player);
    }
}