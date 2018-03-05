using System.Collections.Generic;
using BlackjackSimulator.Models;

namespace BlackjackSimulator.Repositories.Interfaces
{
    public interface IPlayerSimulationStatisticsRepository
    {
        void Save(PlayerSimulationsStatistics playerSimulationsStatistics);
        void Save(List<PlayerSimulationsStatistics> playerSimulationsStatisticsCollection);
    }
}