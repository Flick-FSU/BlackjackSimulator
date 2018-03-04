namespace BlackjackSimulator.Models
{
    public class PlayerSimulationsStatistics
    {
        public int ID { get; set; }
        public string StrategyName { get; set; }
        public decimal AverageBet { get; set; }
        public decimal StartingCash { get; set; }
        public int RunCount { get; set; }
        public decimal AverageCountOfHandsUntilBroke { get; set; }
        public decimal AverageMoneyLostPerHand { get; set; }
        public decimal LossRate { get; set; }
        public decimal WonHandsPercent { get; set; }
        public decimal LostHandsPercent { get; set; }
        public decimal PushHandsPercent { get; set; }
    }
}