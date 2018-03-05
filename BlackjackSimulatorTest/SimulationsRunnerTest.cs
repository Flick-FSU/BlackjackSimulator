using System;
using System.Collections.Generic;
using System.Threading;
using BlackjackSimulator.Entities;
using BlackjackSimulator.Entities.Interfaces;
using BlackjackSimulator.Enums;
using BlackjackSimulator.Models;
using BlackjackSimulator.Repositories.Interfaces;
using BlackjackSimulator.Strategies;
using BlackjackSimulator.Strategies.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace BlackjackSimulatorTest
{
    [TestClass]
    public class SimulationsRunnerTest
    {
        private ISimulationsRunner _sut;

        [TestInitialize]
        public void MyTestInitialize()
        {
            var mockStatisticsRepository = MockRepository.GenerateMock<IPlayerSimulationStatisticsRepository>();
            var mockSimulationsOutputHandler = MockRepository.GenerateMock<ISimulationsOutputHandler>();
            _sut = new SimulationsRunner(mockStatisticsRepository, mockSimulationsOutputHandler);
        }

        [TestMethod]
        public void When_Running_Simulations_Without_Loading_Simulation_Should_Throw_Exception()
        {
            Assert.ThrowsException<InvalidOperationException>(() => _sut.Run(100));
        }

        [TestMethod]
        public void When_Running_Simulations_Should_Save_Simulations_Statistics_In_Repository()
        {
            var mockStatisticsRepository = MockRepository.GenerateMock<IPlayerSimulationStatisticsRepository>();
            var mockSimulationsOutputHandler = MockRepository.GenerateMock<ISimulationsOutputHandler>();
            _sut = new SimulationsRunner(mockStatisticsRepository, mockSimulationsOutputHandler);
            var mockPlayer = MockRepository.GenerateMock<IPlayer>();
            mockPlayer.Stub(mp => mp.HandHistory).Return(new List<IPlayerHand> {GetHandHistoryHand()});
            mockPlayer.Stub(mp => mp.StartingCash).Return(100);
            var simulationProperties = new SimulationProperties
            {
                MaximumBetForTable = 100,
                MinimumBetForTable = 10,
                MaximumPlayersForTable = 2,
                NumberOfDecksInShoe = 2,
                PlayerPropertiesCollection = new List<PlayerProperties>
                {
                    new PlayerProperties { PlayerStrategy = new BasicMinimumPlayerStrategy(), StartingCash = 100 }
                }
            };

            mockStatisticsRepository.Expect(msr => msr.Save(new List<PlayerSimulationsStatistics>())).IgnoreArguments();
            mockStatisticsRepository.Replay();
            _sut.Load(simulationProperties);
            _sut.Run(2);
            
            mockStatisticsRepository.VerifyAllExpectations();
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
