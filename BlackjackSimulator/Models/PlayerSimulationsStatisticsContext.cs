﻿using System.Data.Entity;

namespace BlackjackSimulator.Models
{
    public class PlayerSimulationsStatisticsContext : DbContext
    {
        public PlayerSimulationsStatisticsContext() : base("BlackjackSimulatorDb")
        {
        }

        public DbSet<PlayerSimulationsStatistics> PlayerSimulationsStatisticsCollection { get; set; }
    }
}