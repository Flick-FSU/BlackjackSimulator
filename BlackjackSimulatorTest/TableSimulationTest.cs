using System;
using System.Collections.Generic;
using BlackjackSimulator.Entities;
using BlackjackSimulator.Entities.Interfaces;
using BlackjackSimulator.Models;
using BlackjackSimulator.Strategies.Interfaces;
using GamblingLibrary.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace BlackjackSimulatorTest
{
    [TestClass]
    public class TableSimulationTest
    {
        private Mock<IDealer> _mockDealer;

        [TestInitialize]
        public void MyTestInitialize()
        {
            _mockDealer = new Mock<IDealer>();
        }

        [TestMethod]
        public void When_Table_Simulator_Created_Should_Give_Table_Settings_To_Dealer()
        {
            var tableSettings = new TableSettings(10, 100, 1);
            var dealer = new Dealer(new Mock<IGroupOfCards>().Object,
                new Mock<IGameManager>().Object, new Mock<IDealerStrategy>().Object);
            var sut = new TableSimulation(dealer, tableSettings);

            Assert.AreSame(tableSettings, dealer.TableSettings);
        }

        [TestMethod]
        public void When_Seating_Player_And_Already_At_Table_Maximum_Should_Throw_Exception()
        {
            var sut = GetTableSimulationWithMaximumPlayerCountOf(0);
            var mockPlayer = new Mock<IPlayer>();

            Assert.ThrowsException<OverflowException>(() => sut.Seat(mockPlayer.Object));
        }

        [TestMethod]
        public void When_Seating_Player_Should_Register_Player_With_Dealer()
        {
            var sut = GetTableSimulationWithMaximumPlayerCountOf(1);
            var mockPlayer = new Mock<IPlayer>();
            sut.Seat(mockPlayer.Object);

            mockPlayer.Verify(mp => mp.JoinTableWith(_mockDealer.Object));
        }

        [TestMethod]
        public void When_Seating_Player_Should_Add_Player_To_Seated_Players_Collection()
        {
            var sut = GetTableSimulationWithMaximumPlayerCountOf(1);
            var mockPlayer = new Mock<IPlayer>();
            sut.Seat(mockPlayer.Object);

            Assert.IsTrue(sut.SeatedPlayers.Contains(mockPlayer.Object));
        }

        [TestMethod]
        public void When_Running_Simulation_With_No_Seated_Players_Should_Throw_Exception()
        {
            var sut = GetTableSimulationWithMaximumPlayerCountOf(1);
            Assert.ThrowsException<InvalidOperationException>(() => sut.RunSimulationUntilAllPlayersUnregister());
        }

        [TestMethod]
        public void When_Simulation_Run_Should_Return_All_Seated_Players()
        {
            var sut = GetTableSimulationWithMaximumPlayerCountOf(1);
            var mockPlayer = new Mock<IPlayer>();
            sut.Seat(mockPlayer.Object);
            _mockDealer.Setup(md => md.RegisteredPlayers).Returns(new List<IPlayer>());
            var players = sut.RunSimulationUntilAllPlayersUnregister();

            Assert.AreSame(sut.SeatedPlayers, players);
        }

        [TestMethod]
        public void When_Running_Simulation_Should_Tell_Dealer_To_Play_Until_Registered_Players_Are_Empty()
        {
            var strictDealerMock = new Mock<IDealer>(MockBehavior.Strict);
            var tableSettings = new TableSettings(10, 100, 2);
            strictDealerMock.Setup(sdm => sdm.SetTableSettingsWith(tableSettings));
            var sut = new TableSimulation(strictDealerMock.Object, tableSettings);
            var mockPlayer = new Mock<IPlayer>();
            sut.Seat(mockPlayer.Object);

            strictDealerMock.SetupSequence(sdm => sdm.RegisteredPlayers)
                .Returns(new List<IPlayer> {mockPlayer.Object})
                .Returns(new List<IPlayer>());
            strictDealerMock.Setup(sdm => sdm.PlaySingleGame());

            sut.RunSimulationUntilAllPlayersUnregister();
        }

        private TableSimulation GetTableSimulationWithMaximumPlayerCountOf(int tableMaximum)
        {
            var tableSettings = new TableSettings(10, 100, tableMaximum);
            var sut = new TableSimulation(_mockDealer.Object, tableSettings);
            return sut;
        }
    }
}
