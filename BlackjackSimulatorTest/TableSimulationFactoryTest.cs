using System;
using System.Collections.Generic;
using BlackjackSimulator.Entities;
using BlackjackSimulator.Entities.Interfaces;
using BlackjackSimulator.Models;
using BlackjackSimulator.Strategies;
using BlackjackSimulator.Strategies.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace BlackjackSimulatorTest
{
    [TestClass]
    public class TableSimulationFactoryTest
    {
        private readonly TableSimulationFactory _tableSimulationFactory;

        public TableSimulationFactoryTest()
        {
            _tableSimulationFactory = new TableSimulationFactory();
        }

        [TestMethod]
        public void When_Table_Minimum_Bet_Greater_Than_Table_Maximum_Bet_Should_Throw_Exception()
        {
            var simulationProperties = new SimulationProperties
            {
                MaximumBetForTable = 5,
                MaximumPlayersForTable = 2,
                MinimumBetForTable = 10,
                NumberOfDecksInShoe = 2,
                PlayerPropertiesCollection = new List<PlayerProperties> { new PlayerProperties {PlayerStrategy = new BasicMinimumPlayerStrategy(), StartingCash = 100 } }
            };

            Assert.ThrowsException<ArgumentException>(() => _tableSimulationFactory.CreateTableSimulationFrom(simulationProperties));
        }

        [TestMethod]
        public void When_Table_Maximum_Players_Less_Than_One_Should_Throw_Exception()
        {
            var simulationProperties = new SimulationProperties
            {
                MaximumBetForTable = 10,
                MaximumPlayersForTable = 0,
                MinimumBetForTable = 5,
                NumberOfDecksInShoe = 2,
                PlayerPropertiesCollection = new List<PlayerProperties> { new PlayerProperties { PlayerStrategy = new BasicMinimumPlayerStrategy(), StartingCash = 100 } }
            };

            Assert.ThrowsException<ArgumentException>(() => _tableSimulationFactory.CreateTableSimulationFrom(simulationProperties));
        }

        [TestMethod]
        public void When_Player_Properties_Collection_Is_Empty_Should_Throw_Exception()
        {
            var simulationProperties = new SimulationProperties
            {
                MaximumBetForTable = 10,
                MaximumPlayersForTable = 2,
                MinimumBetForTable = 5,
                NumberOfDecksInShoe = 2,
                PlayerPropertiesCollection = new List<PlayerProperties>()
            };

            Assert.ThrowsException<ArgumentException>(() => _tableSimulationFactory.CreateTableSimulationFrom(simulationProperties));
        }

        [TestMethod]
        public void When_Number_Of_Decks_Less_Than_One_Should_Throw_Exception()
        {
            var simulationProperties = new SimulationProperties
            {
                MaximumBetForTable = 10,
                MaximumPlayersForTable = 0,
                MinimumBetForTable = 5,
                NumberOfDecksInShoe = 0,
                PlayerPropertiesCollection = new List<PlayerProperties> { new PlayerProperties { PlayerStrategy = new BasicMinimumPlayerStrategy(), StartingCash = 100 } }
            };

            Assert.ThrowsException<ArgumentException>(() => _tableSimulationFactory.CreateTableSimulationFrom(simulationProperties));

        }

        [TestMethod]
        public void When_Given_Valid_Simulation_Properties_Should_Return_Table_Simulation_Object()
        {
            var simulationProperties = new SimulationProperties
            {
                MaximumBetForTable = 10,
                MaximumPlayersForTable = 2,
                MinimumBetForTable = 5,
                NumberOfDecksInShoe = 1,
                PlayerPropertiesCollection = new List<PlayerProperties> { new PlayerProperties { PlayerStrategy = new BasicMinimumPlayerStrategy(), StartingCash = 100 } }
            };

            Assert.IsInstanceOfType(_tableSimulationFactory.CreateTableSimulationFrom(simulationProperties), typeof(ITableSimulation));
        }
    }
}
