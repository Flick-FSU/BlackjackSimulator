using System.Collections.Generic;
using BlackjackSimulator.Models;
using BlackjackSimulator.Repositories.Interfaces;

namespace BlackjackSimulator.Repositories
{
    public class PlayerSimulationStatisticsRepository : IPlayerSimulationStatisticsRepository
    {
        public void Save(PlayerSimulationsStatistics playerSimulationsStatistics)
        {
            using (var db = new PlayerSimulationsStatisticsContext())
            {
                db.PlayerSimulationsStatisticsCollection.Add(playerSimulationsStatistics);
                db.SaveChangesAsync();
            }
        }

        public void Save(List<PlayerSimulationsStatistics> playerSimulationsStatisticsCollection)
        {
            using (var db = new PlayerSimulationsStatisticsContext())
            {
                foreach (var playerSimulationsStatistics in playerSimulationsStatisticsCollection)
                {
                    db.PlayerSimulationsStatisticsCollection.Add(playerSimulationsStatistics);
                    db.SaveChangesAsync();
                }
            }
        }
    }
}