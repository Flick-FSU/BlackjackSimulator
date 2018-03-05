using System;
using BlackjackSimulator.Entities;
using BlackjackSimulator.Repositories;
using BlackjackSimulator.SimulationScenarios;
using GamblingLibrary;
using GamblingLibrary.Enums;

namespace BlackjackSimulator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var simulationRunner = new SimulationsRunner(new PlayerSimulationStatisticsRepository(),
                new SimulationsOutputHandler());
            var simulationScenario = new OneBasicMinimumPlayerScenario();

            simulationRunner.Load(simulationScenario.GetSimulationProperties());
            simulationRunner.Run(1000);

            Console.ReadLine();
        }

        private static void TestShuffle()
        {
            int[,] cardCounts = new int[13,4];
            var deckOfCards = new StandardDeckOfCards(new BlackjackCardValueAssigner());
            int runCount = 100000000;

            for (int i = 0; i < runCount; i++)
            {
                deckOfCards.Shuffle();
                var card = deckOfCards.PullTopCard();
                ++cardCounts[(int) card.Type , (int) card.Suit];
                deckOfCards.Cards.Add(card);
            }

            for (int typeIndex = 0; typeIndex < 13; typeIndex++)
            {
                for (int suitIndex = 0; suitIndex < 4; suitIndex++)
                {
                    decimal cardOdds = (decimal) cardCounts[typeIndex, suitIndex] / (decimal) runCount;
                    Console.WriteLine((CardType) typeIndex + " of " + (CardSuit) suitIndex + ": " + cardOdds.ToString("P3"));
                }
            }
        }

        private static void PrintStandardDeckOfCards()
        {
            var blackjackCardValueAssigner = new BlackjackCardValueAssigner();
            var standardDeckOfCards = new StandardDeckOfCards(blackjackCardValueAssigner);
            Console.WriteLine("Card order out of the pack: ");
            PrintCards(standardDeckOfCards);

            standardDeckOfCards.Shuffle();

            Console.WriteLine("\nShuffled cards:");
            PrintCards(standardDeckOfCards);
        }

        private static void PrintCards(StandardDeckOfCards standardDeckOfCards)
        {
            foreach (var card in standardDeckOfCards.Cards)
                Console.WriteLine(card.ToString());
        }
    }
}
