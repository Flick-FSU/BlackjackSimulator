using System;
using System.Linq;
using BlackjackSimulator.Entities.Interfaces;
using BlackjackSimulator.Models;
using BlackjackSimulator.Strategies;
using BlackjackSimulator.Strategies.Interfaces;
using GamblingLibrary;

namespace BlackjackSimulator.Entities
{
    public class TableSimulationFactory
    {
        public ITableSimulation CreateTableSimulationFrom(SimulationProperties simulationProperties)
        {
            Validate(simulationProperties);

            var tableSettings = new TableSettings(simulationProperties.MinimumBetForTable,
                simulationProperties.MaximumBetForTable, simulationProperties.MaximumPlayersForTable);
            var cardDeck = new ShoeOfStandardDecksOfCards(new BlackjackCardValueAssigner(), simulationProperties.NumberOfDecksInShoe);
            var dealer = new Dealer(cardDeck, new GameManager(), new StandardDealerStrategy());
            var tableSimulation = new TableSimulation(dealer, tableSettings);

            foreach (var playerProperties in simulationProperties.PlayerPropertiesCollection)
            {
                var strategy = (IPlayerStrategy) playerProperties.PlayerStrategy.GetConstructors()[0].Invoke(null);
                var player = new Player(playerProperties.StartingCash, strategy);
                tableSimulation.Seat(player);
            }

            return tableSimulation;
        }

        private void Validate(SimulationProperties simulationProperties)
        {
            if (simulationProperties == null)
                throw new ArgumentNullException(nameof(simulationProperties));
            if (!simulationProperties.PlayerPropertiesCollection.Any())
                throw new ArgumentException("No players added to simulation properties");
            foreach (var playerProperties in simulationProperties.PlayerPropertiesCollection)
            {
                if (playerProperties.PlayerStrategy == null)
                    throw new ArgumentException("No player strategy specified");
            }
            if (simulationProperties.MaximumBetForTable < simulationProperties.MinimumBetForTable)
                throw new ArgumentException("Maximum table bet is less than minimum table bet");
            if (simulationProperties.MaximumPlayersForTable < 1)
                throw new ArgumentException("Maximum players for table must be greater than zero");
            if (simulationProperties.NumberOfDecksInShoe < 1)
                throw new ArgumentException("Number of decks in simulation properties must be greater than zero");
        }
    }
}