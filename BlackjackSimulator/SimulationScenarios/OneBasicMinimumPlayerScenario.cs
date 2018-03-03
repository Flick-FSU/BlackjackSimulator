using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlackjackSimulator.Entities;
using BlackjackSimulator.Enums;
using BlackjackSimulator.Interfaces;
using BlackjackSimulator.Models;
using BlackjackSimulator.Strategies;
using GamblingLibrary;

namespace BlackjackSimulator.SimulationScenarios
{
    public class OneBasicMinimumPlayerScenario : ISimulationScenario
    {
        private const decimal TABLE_MINIMUM_BET = 10;
        private const decimal TABLE_MAXIMUM_BET = 100;
        private const int TABLE_MAX_PLAYERS = 1;
        private const int NUMBER_OF_DECKS_IN_SHOE = 4;
        private const decimal PLAYER_STARTING_CASH = 200;
        private const int NUMBER_OF_RUNS = 500;
        private int _totalHandsPlayed;
        private int _totalHandsWon;
        private int _totalHandsPushed;
        private int _totalHandsLost;
        private decimal _totalStartingMoneyLost;
        private decimal _totalMoneyBet;
        
        public void Run()
        {
            var simulationTasks = new List<Task>();
            for (int runIndex = 0; runIndex < NUMBER_OF_RUNS; runIndex++)
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
            var scenarioResult = GetScenarioData();

            using (var db = new ScenarioPlayerResultContext())
            {
                db.ScenarioResults.Add(scenarioResult);
                db.SaveChanges();
            }

            Print(scenarioResult);
        }

        private static void Print(ScenarioPlayerResult scenarioResult)
        {
            Console.WriteLine("----------Results of One Basic Minimum Player Scenario--------");
            Console.WriteLine("Average number of hands until broke: " +
                              scenarioResult.AverageCountOfHandsUntilBroke.ToString("F5"));
            Console.WriteLine("Average money lost per hand: $" + scenarioResult.AverageMoneyLostPerHand.ToString("F5"));
            Console.WriteLine("Hands won: " + scenarioResult.WonHandsPercent.ToString("P3"));
            Console.WriteLine("Hands lost: " + scenarioResult.LostHandsPercent.ToString("P3"));
            Console.WriteLine("Hands pushed: " + scenarioResult.PushHandsPercent.ToString("P3"));
            Console.WriteLine("Loss rate percentage: " + scenarioResult.LossRate.ToString("F5"));
        }

        private ScenarioPlayerResult GetScenarioData()
        {
            var scenarioResult = new ScenarioPlayerResult
            {
                ScenarioName = GetType().Name,
                AverageBet = _totalMoneyBet / (decimal) _totalHandsPlayed,
                StartingCash = PLAYER_STARTING_CASH,
                RunCount = NUMBER_OF_RUNS,
                AverageCountOfHandsUntilBroke = (decimal) _totalHandsPlayed / (decimal) NUMBER_OF_RUNS,
                AverageMoneyLostPerHand = _totalStartingMoneyLost / (decimal) _totalHandsPlayed,
                WonHandsPercent = (decimal) _totalHandsWon / (decimal) _totalHandsPlayed,
                LostHandsPercent = (decimal) _totalHandsLost / (decimal) _totalHandsPlayed,
                PushHandsPercent = (decimal) _totalHandsPushed / (decimal) _totalHandsPlayed
            };
            scenarioResult.LossRate = scenarioResult.AverageBet / (decimal) PLAYER_STARTING_CASH *
                                      scenarioResult.AverageCountOfHandsUntilBroke;
            
            return scenarioResult;
        }

        private void ExecuteSimulationSteps(int runIndex)
        {
            var tableSimulation = GetTableSimulationRun();
            var finishedPlayer = tableSimulation.RunSimulationUntilAllPlayersUnregister()[0];
            Console.Out.WriteLine("Simulation " + (runIndex + 1) + " ended in " + finishedPlayer.HandHistory.Count +
                                  " hands. Max cash: " + finishedPlayer.HandHistory
                                      .Max(hh => hh.TotalPlayerCashAfterOutcome).ToString("C"));
            _totalHandsPlayed += finishedPlayer.HandHistory.Count;
            _totalHandsWon += finishedPlayer.HandHistory.Count(hh => hh.Outcome == HandOutcome.Won);
            _totalHandsLost += finishedPlayer.HandHistory.Count(hh => hh.Outcome == HandOutcome.Lost);
            _totalHandsPushed += finishedPlayer.HandHistory.Count(hh => hh.Outcome == HandOutcome.Pushed);
            _totalStartingMoneyLost += PLAYER_STARTING_CASH - finishedPlayer.TotalCash;

            _totalMoneyBet += finishedPlayer.HandHistory.Sum(hh => hh.Bet);

            //_simulationEndedFlags[runIndex] = true;
        }

        private TableSimulation GetTableSimulationRun()
        {
            var tableSettings = new TableSettings(TABLE_MINIMUM_BET, TABLE_MAXIMUM_BET, TABLE_MAX_PLAYERS);
            var cardDeck = new ShoeOfStandardDecksOfCards(new BlackjackCardValueAssigner(), NUMBER_OF_DECKS_IN_SHOE);
            var dealer = new Dealer(cardDeck, new GameManager(), new StandardDealerStrategy());
            var tableSimulation = new TableSimulation(dealer, tableSettings);
            var player = new Player(PLAYER_STARTING_CASH, new BasicMinimumPlayerStrategy());
            tableSimulation.Seat(player);

            return tableSimulation;
        }
    }
}