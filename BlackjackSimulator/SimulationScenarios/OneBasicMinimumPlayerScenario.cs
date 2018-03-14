using System.Collections.Generic;
using BlackjackSimulator.Models;
using BlackjackSimulator.SimulationScenarios.Interfaces;
using BlackjackSimulator.Strategies;

namespace BlackjackSimulator.SimulationScenarios
{
    public class OneBasicMinimumPlayerScenario : ISimulationScenario
    {
        private const decimal TABLE_MINIMUM_BET = 10;
        private const decimal TABLE_MAXIMUM_BET = 100;
        private const int TABLE_MAX_PLAYERS = 1;
        private const int NUMBER_OF_DECKS_IN_SHOE = 4;
        private const decimal PLAYER_STARTING_CASH = 200;

        public SimulationProperties GetSimulationProperties()
        {
            var simulationProperties = new SimulationProperties
            {
                MaximumBetForTable = TABLE_MAXIMUM_BET,
                MinimumBetForTable = TABLE_MINIMUM_BET,
                MaximumPlayersForTable = TABLE_MAX_PLAYERS,
                NumberOfDecksInShoe = NUMBER_OF_DECKS_IN_SHOE,
                PlayerPropertiesCollection = new List<PlayerProperties>
                {
                    new PlayerProperties
                    {
                        PlayerStrategy = new BasicMinimumPlayerStrategy().GetType(), StartingCash = PLAYER_STARTING_CASH
                    }
                }
            };

            return simulationProperties;
        }
    }
}