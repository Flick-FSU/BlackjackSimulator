using BlackjackSimulator.Entities;
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

        public TableSimulation BuildSimulation()
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