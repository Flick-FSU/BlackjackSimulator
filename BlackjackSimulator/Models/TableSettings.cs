namespace BlackjackSimulator.Models
{
    public class TableSettings
    {
        public decimal MinimumBet { get; }
        public decimal MaximumBet { get; }
        public int MaximumPlayerCount { get; }

        public TableSettings(decimal minimumBet, decimal maximumBet, int maximumPlayerCount)
        {
            MinimumBet = minimumBet;
            MaximumBet = maximumBet;
            MaximumPlayerCount = maximumPlayerCount;
        }
    }
}