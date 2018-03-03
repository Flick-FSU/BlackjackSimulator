using System;
using System.Collections.Generic;
using BlackjackSimulator;
using BlackjackSimulator.Entities;
using BlackjackSimulator.Interfaces;
using BlackjackSimulator.Models;
using BlackjackSimulator.Strategies;
using GamblingLibrary;
using GamblingLibrary.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace BlackjackSimulatorTest
{
    [TestClass]
    public class TableSimulationTest
    {
        private IDealer _mockDealer;

        public TableSimulationTest()
        {
        }

        [TestInitialize]
        public void MyTestInitialize()
        {
            _mockDealer = MockRepository.GenerateMock<IDealer>();
        }

        [TestMethod]
        public void When_Table_Simulator_Created_Should_Give_Table_Settings_To_Dealer()
        {
            var tableSettings = new TableSettings(10, 100, 1);
            var dealer = new Dealer(MockRepository.GenerateMock<IGroupOfCards>(),
                MockRepository.GenerateMock<IGameManager>(), MockRepository.GenerateMock<IDealerStrategy>());
            var sut = new TableSimulation(dealer, tableSettings);

            Assert.AreSame(tableSettings, dealer.TableSettings);
        }

        [TestMethod]
        public void When_Seating_Player_And_Already_At_Table_Maximum_Should_Throw_Exception()
        {
            var sut = GetTableWithMaximumPlayerCountOf(0);
            var mockPlayer = MockRepository.GenerateMock<IPlayer>();

            Assert.ThrowsException<OverflowException>(() => sut.Seat(mockPlayer));
        }

        [TestMethod]
        public void When_Seating_Player_Should_Register_Player_With_Dealer()
        {
            var sut = GetTableWithMaximumPlayerCountOf(1);
            var mockPlayer = MockRepository.GenerateMock<IPlayer>();
            sut.Seat(mockPlayer);

            mockPlayer.AssertWasCalled(mp => mp.JoinTableWith(_mockDealer));
        }

        [TestMethod]
        public void When_Seating_Player_Should_Add_Player_To_Seated_Players_Collection()
        {
            var sut = GetTableWithMaximumPlayerCountOf(1);
            var mockPlayer = MockRepository.GenerateMock<IPlayer>();
            sut.Seat(mockPlayer);

            Assert.IsTrue(sut.SeatedPlayers.Contains(mockPlayer));
        }

        [TestMethod]
        public void When_Running_Simulation_With_No_Seated_Players_Should_Throw_Exception()
        {
            var sut = GetTableWithMaximumPlayerCountOf(1);
            Assert.ThrowsException<InvalidOperationException>(() => sut.RunSimulationUntilAllPlayersUnregister());
        }

        [TestMethod]
        public void When_Simulation_Run_Should_Return_All_Seated_Players()
        {
            var sut = GetTableWithMaximumPlayerCountOf(1);
            var mockPlayer = MockRepository.GenerateMock<IPlayer>();
            sut.Seat(mockPlayer);
            _mockDealer.Stub(md => md.RegisteredPlayers).Return(new List<IPlayer>());
            var players = sut.RunSimulationUntilAllPlayersUnregister();

            Assert.AreSame(sut.SeatedPlayers, players);
        }

        [TestMethod]
        public void When_Running_Simulation_Should_Tell_Dealer_To_Play_Until_Registered_Players_Are_Empty()
        {
            var strictDealerMock = MockRepository.GenerateStrictMock<IDealer>();
            var tableSettings = new TableSettings(10, 100, 2);
            strictDealerMock.Expect(sdm => sdm.SetTableSettingsWith(tableSettings));
            var sut = new TableSimulation(strictDealerMock, tableSettings);
            var mockPlayer = MockRepository.GenerateMock<IPlayer>();
            sut.Seat(mockPlayer);

            strictDealerMock.Expect(sdm => sdm.RegisteredPlayers)
                .Return(new List<IPlayer> {mockPlayer}).Repeat.Once();
            strictDealerMock.Expect(sdm => sdm.PlaySingleGame()).Repeat.Once();
            strictDealerMock.Expect(sdm => sdm.RegisteredPlayers)
                .Return(new List<IPlayer>()).Repeat.Once();

            strictDealerMock.Replay();

            sut.RunSimulationUntilAllPlayersUnregister();
            strictDealerMock.VerifyAllExpectations();
        }

        private TableSimulation GetTableWithMaximumPlayerCountOf(int tableMaximum)
        {
            var tableSettings = new TableSettings(10, 100, tableMaximum);
            var sut = new TableSimulation(_mockDealer, tableSettings);
            return sut;
        }
    }
}
