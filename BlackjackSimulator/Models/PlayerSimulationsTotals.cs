namespace BlackjackSimulator.Models
{
    public class PlayerSimulationsTotals
    {
        public PlayerSimulationsTotals(string strategyName, decimal startingCash)
        {
            StrategyName = strategyName;
            StartingCash = startingCash;
        }

        public string StrategyName { get; }
        public decimal StartingCash { get; }
        public int TotalHandsPlayed;
        public int TotalHandsWon;
        public int TotalHandsPushed;
        public int TotalHandsLost;
        public decimal TotalStartingMoneyLost;
        public decimal TotalMoneyBet;
    }
}