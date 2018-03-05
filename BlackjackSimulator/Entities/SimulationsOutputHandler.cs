using System;
using System.Linq;
using BlackjackSimulator.Entities.Interfaces;
using BlackjackSimulator.Models;

namespace BlackjackSimulator.Entities
{
    public class SimulationsOutputHandler : ISimulationsOutputHandler
    {
        public void Print(PlayerSimulationsStatistics simulationsStatistics)
        {
            Console.WriteLine("----------Results of One Basic Minimum Player Scenario--------");
            Console.WriteLine("Average number of hands until broke: " +
                              simulationsStatistics.AverageCountOfHandsUntilBroke.ToString("F5"));
            Console.WriteLine("Average money lost per hand: $" + simulationsStatistics.AverageMoneyLostPerHand.ToString("F5"));
            Console.WriteLine("Hands won: " + simulationsStatistics.WonHandsPercent.ToString("P3"));
            Console.WriteLine("Hands lost: " + simulationsStatistics.LostHandsPercent.ToString("P3"));
            Console.WriteLine("Hands pushed: " + simulationsStatistics.PushHandsPercent.ToString("P3"));
            Console.WriteLine("Loss rate percentage: " + simulationsStatistics.LossRate.ToString("F5"));
        }

        public void OutputSingleSimulationResult(int runIndex, IPlayer player)
        {
            Console.Out.WriteLine("Simulation " + (runIndex + 1) + " ended in " +
                                  player.HandHistory.Count +
                                  " hands. Max cash: " + player.HandHistory
                                      .Max(hh => hh.TotalPlayerCashAfterOutcome).ToString("C"));
        }
    }
}