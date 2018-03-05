using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlackjackSimulator.Entities.Interfaces;
using BlackjackSimulator.Enums;
using BlackjackSimulator.Models;
using BlackjackSimulator.Repositories.Interfaces;

namespace BlackjackSimulator.Entities
{
    public class SimulationsRunner : ISimulationsRunner
    {
        private readonly IPlayerSimulationStatisticsRepository _mockStatisticsRepository;
        private readonly ISimulationsOutputHandler _simulationsOutputHandler;
        private ITableSimulation _tableSimulation;
        private int _numberOfSimulationRuns;
        private List<PlayerSimulationsTotals> _playerSimulationsTotalsCollection;
        private List<IPlayer> _finishedPlayers;

        public SimulationsRunner(IPlayerSimulationStatisticsRepository mockStatisticsRepository, ISimulationsOutputHandler simulationsOutputHandler)
        {
            _mockStatisticsRepository = mockStatisticsRepository;
            _simulationsOutputHandler = simulationsOutputHandler;
        }

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

            //Task.Factory.ContinueWhenAll(simulationTasks.ToArray(), OutputResults); //This fails unit tests due to race conditions
            //or
            //simulationTasks.ForEach(st => st.Wait());
            //OutputResults();
            //or
            var allSimulationsTask = Task.WhenAll(simulationTasks);
            allSimulationsTask.Wait();
            if (allSimulationsTask.Status == TaskStatus.RanToCompletion)
                OutputResults();
            else
                throw new Exception("One or more threads did not complete");
        }

        private void ExecuteSimulationSteps(int runIndex)
        {
            _finishedPlayers = _tableSimulation.RunSimulationUntilAllPlayersUnregister();
            foreach (var player in _finishedPlayers)
            {
                var playerSimulationsTotals =
                    _playerSimulationsTotalsCollection.ElementAt(_finishedPlayers.IndexOf(player));

                _simulationsOutputHandler.OutputSingleSimulationResult(runIndex, player);

                playerSimulationsTotals.TotalHandsPlayed += player.HandHistory.Count;
                playerSimulationsTotals.TotalHandsWon += player.HandHistory.Count(hh => hh.Outcome == HandOutcome.Won);
                playerSimulationsTotals.TotalHandsLost += player.HandHistory.Count(hh => hh.Outcome == HandOutcome.Lost);
                playerSimulationsTotals.TotalHandsPushed += player.HandHistory.Count(hh => hh.Outcome == HandOutcome.Pushed);
                playerSimulationsTotals.TotalStartingMoneyLost += player.StartingCash - player.CurrentTotalCash;
                playerSimulationsTotals.TotalMoneyBet += player.HandHistory.Sum(hh => hh.Bet);
            }
        }

        public void OutputResults()
        {
            var simulationsStatisticsCollection = GetSimulationsStatistics();
            _mockStatisticsRepository.Save(simulationsStatisticsCollection);

            foreach (var playerSimulationsStatistics in simulationsStatisticsCollection)
                _simulationsOutputHandler.Print(playerSimulationsStatistics);
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
    }
}