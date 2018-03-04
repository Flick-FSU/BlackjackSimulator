using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlackjackSimulator.Enums;
using BlackjackSimulator.Interfaces;
using BlackjackSimulator.Models;

namespace BlackjackSimulator.Entities
{
    public class SimulationsRunner : ISimulationsRunner
    {
        private ITableSimulation _tableSimulation;
        private int _numberOfSimulationRuns;
        private List<PlayerSimulationsTotals> _playerSimulationsTotalsCollection;
        private List<IPlayer> _finishedPlayers;

        public void Load(ITableSimulation tableSimulation)
        {
            _tableSimulation = tableSimulation;
            _playerSimulationsTotalsCollection = new List<PlayerSimulationsTotals>();
            for (int playerIndex = 0; playerIndex < _tableSimulation.NumberOfPlayers; playerIndex++)
                _playerSimulationsTotalsCollection.Add(new PlayerSimulationsTotals());
        }

        public void Run(int numberOfSimulationRuns)
        {
            _numberOfSimulationRuns = numberOfSimulationRuns;
            if (_tableSimulation == null)
                throw new InvalidOperationException("Simulation not loaded");

            var simulationTasks = new List<Task>();
            for (int runIndex = 0; runIndex < numberOfSimulationRuns; runIndex++)
            {
                var index = runIndex;
                simulationTasks.Add(Task.Factory.StartNew(() => ExecuteSimulationSteps(index)));
            }

            Task.Factory.ContinueWhenAll(simulationTasks.ToArray(), OutputResults);
            //or
            //simulationTasks.ForEach(st => st.Wait());
            //OutputResults();
            //or
            //await Task.WhenAll(simulationTasks);
            //OutputResults();
        }

        public void OutputResults(Task[] tasks)
        {
            var simulationsStatisticsCollection = GetSimulationsStatistics();

            foreach (var playerSimulationsStatistics in simulationsStatisticsCollection)
            {
                using (var db = new PlayerSimulationsStatisticsContext())
                {
                    db.ScenarioResults.Add(playerSimulationsStatistics);
                    db.SaveChanges();
                }

                Print(playerSimulationsStatistics);
            }
        }

        private void ExecuteSimulationSteps(int runIndex)
        {
            _finishedPlayers = _tableSimulation.RunSimulationUntilAllPlayersUnregister();
            foreach (var player in _finishedPlayers)
            {
                var playerSimulationsTotals =
                    _playerSimulationsTotalsCollection.ElementAt(_finishedPlayers.IndexOf(player));

                OutputSingleSimulationResult(runIndex, player);

                playerSimulationsTotals.TotalHandsPlayed += player.HandHistory.Count;
                playerSimulationsTotals.TotalHandsWon += player.HandHistory.Count(hh => hh.Outcome == HandOutcome.Won);
                playerSimulationsTotals.TotalHandsLost += player.HandHistory.Count(hh => hh.Outcome == HandOutcome.Lost);
                playerSimulationsTotals.TotalHandsPushed += player.HandHistory.Count(hh => hh.Outcome == HandOutcome.Pushed);
                playerSimulationsTotals.TotalStartingMoneyLost += player.StartingCash - player.CurrentTotalCash;

                playerSimulationsTotals.TotalMoneyBet += player.HandHistory.Sum(hh => hh.Bet);
            }
        }

        private List<PlayerSimulationsStatistics> GetSimulationsStatistics()
        {
            var simulationsStatisticsToReturn = new List<PlayerSimulationsStatistics>();
            foreach (var player in _finishedPlayers)
            {
                var playerSimulationsTotals =
                    _playerSimulationsTotalsCollection.ElementAt(_finishedPlayers.IndexOf(player));

                var simulationsStatistics = new PlayerSimulationsStatistics
                {
                    StrategyName = player.StrategyName,
                    AverageBet = playerSimulationsTotals.TotalMoneyBet / (decimal)playerSimulationsTotals.TotalHandsPlayed,
                    StartingCash = player.StartingCash,
                    RunCount = _numberOfSimulationRuns,
                    AverageCountOfHandsUntilBroke = (decimal)playerSimulationsTotals.TotalHandsPlayed / (decimal) _numberOfSimulationRuns,
                    AverageMoneyLostPerHand = playerSimulationsTotals.TotalStartingMoneyLost / (decimal)playerSimulationsTotals.TotalHandsPlayed,
                    WonHandsPercent = (decimal)playerSimulationsTotals.TotalHandsWon / (decimal)playerSimulationsTotals.TotalHandsPlayed,
                    LostHandsPercent = (decimal)playerSimulationsTotals.TotalHandsLost / (decimal)playerSimulationsTotals.TotalHandsPlayed,
                    PushHandsPercent = (decimal)playerSimulationsTotals.TotalHandsPushed / (decimal)playerSimulationsTotals.TotalHandsPlayed
                };
                simulationsStatistics.LossRate = simulationsStatistics.AverageBet / player.StartingCash *
                                          simulationsStatistics.AverageCountOfHandsUntilBroke;

                simulationsStatisticsToReturn.Add(simulationsStatistics);
            }

            return simulationsStatisticsToReturn;
        }

        private static void Print(PlayerSimulationsStatistics simulationsStatistics)
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

        private static void OutputSingleSimulationResult(int runIndex, IPlayer player)
        {
            Console.Out.WriteLine("Simulation " + (runIndex + 1) + " ended in " +
                                  player.HandHistory.Count +
                                  " hands. Max cash: " + player.HandHistory
                                      .Max(hh => hh.TotalPlayerCashAfterOutcome).ToString("C"));
        }
    }
}