using System.Data.Entity;

namespace BlackjackSimulator.Models
{
    public class ScenarioPlayerResultContext : DbContext
    {
        public ScenarioPlayerResultContext() : base("BlackjackSimulatorDb")
        {
        }

        public DbSet<ScenarioPlayerResult> ScenarioResults { get; set; }
    }
}