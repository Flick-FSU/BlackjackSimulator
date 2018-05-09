using System;
using System.Collections.Generic;
using BlackjackSimulator.Entities;
using BlackjackSimulator.Entities.Interfaces;
using BlackjackSimulator.Enums;
using BlackjackSimulator.Models;
using BlackjackSimulator.Repositories.Interfaces;
using BlackjackSimulator.Strategies;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Rhino.Mocks;
using MockRepository = Rhino.Mocks.MockRepository;

namespace BlackjackSimulatorTest
{
    [TestClass]
    public class SimulationsRunnerTest
    {
        private ISimulationsRunner _sut;

        [TestInitialize]
        public void MyTestInitialize()
        {
            var mockStatisticsRepository = new Mock<IPlayerSimulationStatisticsRepository>();
            var mockSimulationsOutputHandler = new Mock<ISimulationsOutputHandler>();
            var mockTableSimulationFactory = new Mock<ITableSimulationFactory>();
            _sut = new SimulationsRunner(mockStatisticsRepository.Object, mockSimulationsOutputHandler.Object, mockTableSimulationFactory.Object);
        }

        [TestMethod]
        public void When_Running_Simulations_Without_Loading_Simulation_Should_Throw_Exception()
        {
            Assert.ThrowsException<InvalidOperationException>(() => _sut.Run(100));
        }

        [TestMethod]
        public void When_Running_Simulations_Should_Save_Simulations_Statistics_In_Repository()
        {
            var mockStatisticsRepository = new Mock<IPlayerSimulationStatisticsRepository>();
            var mockSimulationsOutputHandler = new Mock<ISimulationsOutputHandler>();
            _sut = new SimulationsRunner(mockStatisticsRepository.Object, mockSimulationsOutputHandler.Object, new TableSimulationFactory());
            var mockPlayer = new Mock<IPlayer>();
            mockPlayer.Setup(mp => mp.HandHistory).Returns(new List<IPlayerHand> {GetHandHistoryHand()});
            mockPlayer.Setup(mp => mp.StartingCash).Returns(100);
            var simulationProperties = new SimulationProperties
            {
                MaximumBetForTable = 100,
                MinimumBetForTable = 10,
                MaximumPlayersForTable = 2,
                NumberOfDecksInShoe = 2,
                PlayerPropertiesCollection = new List<PlayerProperties>
                {
                    new PlayerProperties { PlayerStrategy = new BasicMinimumPlayerStrategy().GetType(), StartingCash = 100 }
                }
            };

            _sut.Load(simulationProperties);
            _sut.Run(2);

            mockStatisticsRepository.Verify(msr => msr.Save(It.IsAny<List<PlayerSimulationsStatistics>>()));
        }

        private IPlayerHand GetHandHistoryHand()
        {
            return new PlayerHand
            {
                Bet = 10,
                Outcome = HandOutcome.Won,
                TotalPlayerCashAfterOutcome = 100
            };
        }
    }
}
