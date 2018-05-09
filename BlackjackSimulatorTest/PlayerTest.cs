using System;
using System.Collections.Generic;
using System.Linq;
using BlackjackSimulator.Entities;
using BlackjackSimulator.Entities.Interfaces;
using BlackjackSimulator.Models;
using BlackjackSimulator.Strategies.Interfaces;
using GamblingLibrary;
using GamblingLibrary.Enums;
using GamblingLibrary.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Rhino.Mocks;
using MockRepository = Rhino.Mocks.MockRepository;

namespace BlackjackSimulatorTest
{
    [TestClass]
    public class PlayerTest
    {
        private const int TOTAL_CASH_AMOUNT = 500;
        private IPlayer _sut;
        private Mock<IPlayerStrategy> _mockPlayerStrategy;
        private readonly ICardValueAssigner _blackjackCardValueAssigner;
        private readonly ICard _nullCard;
        private readonly TableSettings _tableSettings;

        public PlayerTest()
        {
            _blackjackCardValueAssigner = new BlackjackCardValueAssigner();
            _nullCard = new NullCard();
            _tableSettings = new TableSettings(10, 500, 4);
        }

        [TestInitialize]
        public void MyTestInitialize()
        {
            _mockPlayerStrategy = new Mock<IPlayerStrategy>();
            _sut = new Player(TOTAL_CASH_AMOUNT, _mockPlayerStrategy.Object);
        }

        [TestMethod]
        public void When_Initializing_Player_Should_Not_Be_Requesting_New_Card()
        {
            Assert.IsFalse(_sut.DoesNeedCard);
        }

        [TestMethod]
        public void When_Initializing_Player_Starting_Cash_Should_Equal_Current_Cash()
        {
            Assert.AreEqual(_sut.CurrentTotalCash, _sut.StartingCash);
        }

        [TestMethod]
        public void When_Taking_A_Card_Should_Throw_Exception_If_No_Bet_Is_Placed_Or_Hand_In_Play()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => _sut.TakeCard(_nullCard));
        }

        [TestMethod]
        public void When_Taking_A_Card_Should_Increment_Card_Count_By_One()
        {
            _sut.JoinTableWith(GetMockDealerWith(_tableSettings).Object);
            _sut.PlaceInitialBet();
            _sut.TakeCard(_nullCard);

            Assert.AreEqual(1, _sut.CurrentHands[0].Cards.Count);
        }

        [TestMethod]
        public void When_Joining_Table_With_Dealer_Should_Register_With_Dealer()
        {
            var mockDealer = new Mock<IDealer>();
            _sut.JoinTableWith(mockDealer.Object);

            mockDealer.Verify(md => md.Register(_sut));
        }

        [TestMethod]
        public void When_Placing_Initial_Bet_Without_Joining_Table_Should_Throw_Exception()
        {
            Assert.ThrowsException<NullReferenceException>(() => _sut.PlaceInitialBet());
        }

        [TestMethod]
        public void When_Initial_Bet_Placed_Should_Have_Only_One_Hand()
        {
            _sut.JoinTableWith(GetMockDealerWith(_tableSettings).Object);
            _sut.PlaceInitialBet();
            Assert.AreEqual(1, _sut.CurrentHands.Count);
        }

        [TestMethod]
        public void When_Initial_Bet_Placed_Should_Have_Hand_With_No_Cards()
        {
            _sut.JoinTableWith(GetMockDealerWith(_tableSettings).Object);
            _sut.PlaceInitialBet();
            Assert.IsFalse(_sut.CurrentHands[0].Cards.Any());
        }

        [TestMethod]
        public void When_Initial_Bet_Placed_Should_Remove_Bet_Amount_From_Total_Cash()
        {
            decimal initialTotalCash = _sut.CurrentTotalCash;
            _mockPlayerStrategy.Setup(mps => mps.GetInitialBetAmount(It.IsAny<IPlayerHand>(), It.IsAny<decimal>(), It.IsAny<TableSettings>()))
                .Returns(_tableSettings.MinimumBet);
            _sut.JoinTableWith(GetMockDealerWith(_tableSettings).Object);
            _sut.PlaceInitialBet();
            Assert.AreEqual(initialTotalCash - _sut.CurrentTotalCash, _sut.CurrentHands[0].Bet);
        }

        [TestMethod]
        public void When_Playing_Turn_Without_A_Hand_Should_Throw_Exception()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => _sut.PlayTurn(_nullCard));
        }

        [TestMethod]
        public void When_Player_Receives_Requested_Card_Should_No_Longer_Be_Signalling_For_New_Card()
        {
            var playerHand = new PlayerHand();
            _mockPlayerStrategy.SetupSequence(mpb => mpb.ShouldHit(playerHand, _nullCard)).Returns(true);
            _sut.CurrentHands.Add(playerHand);

            _sut.PlayTurn(_nullCard);
            _sut.TakeCard(_nullCard);

            Assert.IsFalse(_sut.DoesNeedCard);
        }

        [TestMethod]
        public void When_Player_Strategy_Says_Should_Hit_Should_Signal_For_New_Card()
        {
            var playerHand = new PlayerHand();
            _mockPlayerStrategy.Setup(mpb => mpb.ShouldHit(playerHand, _nullCard)).Returns(true);
            _sut.CurrentHands.Add(playerHand);

            _sut.PlayTurn(_nullCard);
            Assert.IsTrue(_sut.DoesNeedCard);
        }

        [TestMethod]
        public void When_Player_Strategy_Says_Should_Hit_And_Player_Has_Busted_Should_Not_Signal_For_New_Card()
        {
            var playerHand = new PlayerHand();
            var nineCard = new Card(CardType.Nine, CardSuit.Clubs, _blackjackCardValueAssigner);
            playerHand.Cards.AddRange(new List<ICard> { nineCard, nineCard, nineCard });
            _mockPlayerStrategy.Setup(mpb => mpb.ShouldHit(playerHand, _nullCard)).Returns(true);
            _sut.CurrentHands.Add(playerHand);

            _sut.PlayTurn(_nullCard);
            Assert.IsFalse(_sut.DoesNeedCard);
        }

        [TestMethod]
        public void When_Player_Strategy_Says_Should_Not_Hit_Should_Not_Signal_For_New_Card()
        {
            var playerHand = new PlayerHand();
            _mockPlayerStrategy.Setup(mpb => mpb.ShouldHit(playerHand, _nullCard)).Returns(false);
            _sut.CurrentHands.Add(playerHand);

            _sut.PlayTurn(_nullCard);
            Assert.IsFalse(_sut.DoesNeedCard);
        }

        [TestMethod]
        public void When_Player_Strategy_Says_Should_Hit_And_Player_Has_Split_Aces_Should_Not_Signal_For_New_Card()
        {
            var playerHand = new PlayerHand();
            playerHand.Cards.Add(new Card(CardType.Ace, CardSuit.Clubs, _blackjackCardValueAssigner));
            playerHand.Cards.Add(new Card(CardType.Ace, CardSuit.Clubs, _blackjackCardValueAssigner));
            _mockPlayerStrategy.Setup(mps => mps.ShouldSplit(playerHand, _nullCard)).Returns(true);
            _sut.CurrentHands.Add(playerHand);
            _sut.PlayTurn(_nullCard);
            _sut.TakeCard(_nullCard);
            _mockPlayerStrategy.Setup(mpb => mpb.ShouldHit(_sut.InPlayHand, _nullCard)).Returns(true);

            _sut.PlayTurn(_nullCard);
            Assert.IsFalse(_sut.DoesNeedCard);
        }

        [TestMethod]
        public void When_Player_Strategy_Says_Should_Hit_And_Player_Has_Split_Non_Aces_Should_Signal_For_New_Card()
        {
            GivePlayerSplittableHand(_nullCard);
            _sut.PlayTurn(_nullCard);
            _sut.TakeCard(_nullCard);
            _mockPlayerStrategy.Setup(mpb => mpb.ShouldHit(_sut.InPlayHand, _nullCard)).Returns(true);

            _sut.PlayTurn(_nullCard);
            Assert.IsTrue(_sut.DoesNeedCard);
        }

        [TestMethod]
        public void When_Player_Strategy_Says_Should_Hit_And_Player_Has_Already_Doubled_Down_Should_Not_Signal_For_New_Card()
        {
            GivePlayerDoubleDownableHand(_nullCard);
            _sut.PlayTurn(_nullCard);
            _sut.TakeCard(_nullCard);
            _mockPlayerStrategy.Setup(mpb => mpb.ShouldHit(_sut.InPlayHand, _nullCard)).Returns(true);

            _sut.PlayTurn(_nullCard);
            Assert.IsFalse(_sut.DoesNeedCard);
        }

        [TestMethod]
        public void When_Player_Strategy_Says_Should_Split_And_Player_Has_The_Money_Should_Signal_For_New_Card()
        {
            GivePlayerSplittableHand(_nullCard);

            _sut.PlayTurn(_nullCard);
            Assert.IsTrue(_sut.DoesNeedCard);
        }

        [TestMethod]
        public void When_Player_Strategy_Says_Should_Split_And_Player_Has_Busted_Should_Not_Signal_For_New_Card()
        {
            var playerHand = new PlayerHand();
            var nineCard = new Card(CardType.Nine, CardSuit.Clubs, _blackjackCardValueAssigner);
            playerHand.Cards.AddRange(new List<ICard> { nineCard, nineCard, nineCard });
            _mockPlayerStrategy.Setup(mpb => mpb.ShouldSplit(playerHand, _nullCard)).Returns(true);
            _sut.CurrentHands.Add(playerHand);

            _sut.PlayTurn(_nullCard);
            Assert.IsFalse(_sut.DoesNeedCard);
        }

        [TestMethod]
        public void When_Player_Strategy_Says_Should_Split_And_Player_Has_The_Money_Should_Call_For_Hand_Split()
        {
            const decimal betAmount = TOTAL_CASH_AMOUNT / 10;
            GivePlayerMockSplittableHand(_nullCard, betAmount);

            _sut.PlayTurn(_nullCard);
            _sut.InPlayHand.AssertWasCalled(ch => ch.Split());
        }

        [TestMethod]
        public void When_Player_Strategy_Says_Should_Split_And_Player_Does_Not_Have_The_Money_Should_Not_Call_For_Hand_Split()
        {
            const decimal betAmount = TOTAL_CASH_AMOUNT + 1;
            GivePlayerMockSplittableHand(_nullCard, betAmount);

            _sut.PlayTurn(_nullCard);
            _sut.InPlayHand.AssertWasNotCalled(ch => ch.Split());
        }

        [TestMethod]
        public void When_Player_Strategy_Says_Should_Split_And_Player_Has_The_Money_Should_Create_Another_Hand()
        {
            GivePlayerSplittableHand(_nullCard);

            _sut.PlayTurn(_nullCard);
            Assert.AreEqual(2, _sut.CurrentHands.Count);
        }

        [TestMethod]
        public void When_Player_Tries_To_Split_Two_Different_Card_Types_Should_Not_Signal_For_New_Card()
        {
            var mockPlayBehavior = new Mock<IPlayerStrategy>();
            var playerHand = new PlayerHand { Bet = TOTAL_CASH_AMOUNT / 10 };
            playerHand.Cards.Add(new Card(CardType.Eight, CardSuit.Clubs, _blackjackCardValueAssigner));
            playerHand.Cards.Add(new Card(CardType.Nine, CardSuit.Clubs, _blackjackCardValueAssigner));
            mockPlayBehavior.Setup(mpb => mpb.ShouldSplit(playerHand, _nullCard)).Returns(true);
            var sut = new Player(TOTAL_CASH_AMOUNT, mockPlayBehavior.Object);
            sut.CurrentHands.Add(playerHand);

            Assert.IsFalse(sut.DoesNeedCard);
        }

        [TestMethod]
        public void When_Player_Splits_Should_Have_One_Card_In_Current_Hand()
        {
            GivePlayerSplittableHand(_nullCard);

            _sut.PlayTurn(_nullCard);
            Assert.AreEqual(1, _sut.InPlayHand.Cards.Count);
        }

        [TestMethod]
        public void When_Player_Splits_Should_Have_One_Card_In_Last_Hand()
        {
            GivePlayerSplittableHand(_nullCard);

            _sut.PlayTurn(_nullCard);
            Assert.AreEqual(1, _sut.CurrentHands.Last().Cards.Count);
        }

        [TestMethod]
        public void When_Player_Splits_Should_Have_Same_Bet_Size_In_Both_Split_Hands()
        {
            GivePlayerSplittableHand(_nullCard);

            _sut.PlayTurn(_nullCard);
            Assert.AreEqual(_sut.InPlayHand.Bet, _sut.CurrentHands.Last().Bet);
        }

        [TestMethod]
        public void When_Player_Splits_Should_Reduce_Total_Cash_Amount_By_Bet_Amount_Of_Current_Hand()
        {
            GivePlayerSplittableHand(_nullCard);
            var initialTotalCash = _sut.CurrentTotalCash;

            _sut.PlayTurn(_nullCard);
            Assert.AreEqual(initialTotalCash - _sut.InPlayHand.Bet, _sut.CurrentTotalCash);
        }

        [TestMethod]
        public void When_Player_Splits_Should_Set_Split_Flag()
        {
            GivePlayerSplittableHand(_nullCard);

            _sut.PlayTurn(_nullCard);
            Assert.IsTrue(_sut.InPlayHand.IsASplit);
        }

        [TestMethod]
        public void When_Player_Strategy_Says_To_Double_Down_And_Player_Has_The_Money_Should_Request_New_Card()
        {
            GivePlayerDoubleDownableHand(_nullCard);

            _sut.PlayTurn(_nullCard);
            Assert.IsTrue(_sut.DoesNeedCard);
        }

        [TestMethod]
        public void When_Player_Strategy_Says_To_Double_Down_And_Player_Has_The_Money_Should_Double_Current_Hand_Bet()
        {
            GivePlayerDoubleDownableHand(_nullCard);
            var currentBetAmount = _sut.InPlayHand.Bet;

            _sut.PlayTurn(_nullCard);
            Assert.AreEqual(currentBetAmount + currentBetAmount, _sut.InPlayHand.Bet);
        }

        [TestMethod]
        public void When_Player_Strategy_Says_To_Double_Down_And_Player_Does_Not_Have_The_Money_Current_Hand_Bet_Should_Stay_The_Same()
        {
            GivePlayerDoubleDownableHand(_nullCard);
            _sut.InPlayHand.Bet = TOTAL_CASH_AMOUNT + 1;
            var currentBetAmount = _sut.InPlayHand.Bet;

            _sut.PlayTurn(_nullCard);
            Assert.AreEqual(currentBetAmount, _sut.InPlayHand.Bet);
        }

        [TestMethod]
        public void When_Player_Strategy_Says_Should_Double_Down_And_Player_Has_Busted_Should_Not_Signal_For_New_Card()
        {
            var playerHand = new PlayerHand();
            var nineCard = new Card(CardType.Nine, CardSuit.Clubs, _blackjackCardValueAssigner);
            playerHand.Cards.AddRange(new List<ICard> { nineCard, nineCard, nineCard });
            _mockPlayerStrategy.Setup(mpb => mpb.ShouldDoubleDown(playerHand, _nullCard)).Returns(true);
            _sut.CurrentHands.Add(playerHand);

            _sut.PlayTurn(_nullCard);
            Assert.IsFalse(_sut.DoesNeedCard);
        }

        [TestMethod]
        public void When_Trying_To_Double_Down_On_Hand_That_Has_Already_Doubled_Down_Should_Not_Ask_For_New_Card()
        {
            GivePlayerDoubleDownableHand(_nullCard);
            _sut.PlayTurn(_nullCard);
            _sut.TakeCard(_nullCard);
            _sut.PlayTurn(_nullCard);

            Assert.IsFalse(_sut.DoesNeedCard);
        }

        [TestMethod]
        public void When_Trying_To_Double_Down_On_Hand_That_Has_Already_Doubled_Down_Should_Not_Double_Bet_Again()
        {
            GivePlayerDoubleDownableHand(_nullCard);
            _sut.PlayTurn(_nullCard);
            var betAfterFirstDoubleDown = _sut.InPlayHand.Bet;
            _sut.TakeCard(_nullCard);
            _sut.PlayTurn(_nullCard);

            Assert.AreEqual(betAfterFirstDoubleDown, _sut.InPlayHand.Bet);
        }

        [TestMethod]
        public void When_Doubling_Down_On_Hand_Should_Set_Double_Down_Flag()
        {
            GivePlayerDoubleDownableHand(_nullCard);

            _sut.PlayTurn(_nullCard);
            Assert.IsTrue(_sut.InPlayHand.IsADoubleDown);
        }

        [TestMethod]
        public void When_Trying_To_Double_Down_On_Hand_That_Has_Split_Aces_Should_Not_Ask_For_New_Card()
        {
            GivePlayerSplittableHand(_nullCard);
            _sut.PlayTurn(_nullCard);
            _sut.TakeCard(_nullCard);
            _mockPlayerStrategy.Setup(mps => mps.ShouldDoubleDown(_sut.InPlayHand, _nullCard)).Returns(true);
            _sut.PlayTurn(_nullCard);

            Assert.IsFalse(_sut.DoesNeedCard);
        }

        [TestMethod]
        public void When_Trying_To_Double_Down_On_Hand_That_Has_Split_Aces_Should_Not_Double_Bet()
        {
            GivePlayerSplittableHand(_nullCard);
            _sut.PlayTurn(_nullCard);
            var betAfterSplit = _sut.InPlayHand.Bet;
            _sut.TakeCard(_nullCard);
            _mockPlayerStrategy.Setup(mps => mps.ShouldDoubleDown(_sut.InPlayHand, _nullCard)).Returns(true);
            _sut.PlayTurn(_nullCard);

            Assert.AreEqual(betAfterSplit, _sut.InPlayHand.Bet);
        }

        [TestMethod]
        public void When_Player_Has_Two_Hands_Should_Play_The_Second_When_The_First_Is_Finished()
        {
            var mockPlayerStrategy = new Mock<IPlayerStrategy>();
            var sut = new Player(TOTAL_CASH_AMOUNT, mockPlayerStrategy.Object);
            var playerHand1 = new PlayerHand();
            var playerHand2 = new PlayerHand();
            sut.CurrentHands.Add(playerHand1);
            sut.CurrentHands.Add(playerHand2);

            sut.PlayTurn(_nullCard);
            Assert.AreSame(playerHand2, sut.InPlayHand);
        }

        [TestMethod]
        public void When_Player_Created_Should_Not_Be_At_A_Table()
        {
            Assert.IsFalse(_sut.IsAtTable);
        }

        [TestMethod]
        public void When_Joined_Table_With_Dealer_Player_Should_Indicate_Is_At_Table()
        {
            var mockDealer = new Mock<IDealer>();
            _sut.JoinTableWith(mockDealer.Object);

            Assert.IsTrue(_sut.IsAtTable);
        }

        [TestMethod]
        public void When_Player_Strategy_Says_To_Stay_At_Table_Player_Should_Indicate_Is_At_Table()
        {
            var tableSettings = new TableSettings(10, 500, 4);
            _sut.JoinTableWith(GetMockDealerWith(tableSettings).Object);
            _mockPlayerStrategy.Setup(mps => mps.ShouldLeaveTable(_sut.CurrentTotalCash, tableSettings)).Returns(false);

            _sut.LeaveTableOrStay();
            Assert.IsTrue(_sut.IsAtTable);
        }

        [TestMethod]
        public void When_Player_Strategy_Says_To_Leave_Table_Should_Unregister_With_Dealer()
        {
            var mockDealer = GetMockDealerWith(_tableSettings);
            _sut.JoinTableWith(mockDealer.Object);
            _mockPlayerStrategy.Setup(mps => mps.ShouldLeaveTable(_sut.CurrentTotalCash, _tableSettings)).Returns(true);

            _sut.LeaveTableOrStay();

            mockDealer.Verify(md => md.Unregister(_sut));
        }

        [TestMethod]
        public void When_Player_Strategy_Says_To_Leave_Table_Player_Should_Indicate_Is_Not_At_Table()
        {
            _sut.JoinTableWith(GetMockDealerWith(_tableSettings).Object);
            _mockPlayerStrategy.Setup(mps => mps.ShouldLeaveTable(_sut.CurrentTotalCash, _tableSettings)).Returns(true);

            _sut.LeaveTableOrStay();
            Assert.IsFalse(_sut.IsAtTable);
        }

        [TestMethod]
        public void When_Deciding_To_Leave_Table_Or_Stay_Should_Throw_Exception_If_Not_At_Table()
        {
            Assert.ThrowsException<NullReferenceException>(() => _sut.LeaveTableOrStay());
        }

        [TestMethod]
        public void When_Saving_Current_Hands_Should_Increase_Hand_History_Count_By_Current_Hands_Count()
        {
            _sut.CurrentHands.Add(new PlayerHand());
            _sut.CurrentHands.Add(new PlayerHand());
            _sut.SaveCurrentHands();

            Assert.AreEqual(_sut.HandHistory.Count, _sut.CurrentHands.Count);
        }

        [TestMethod]
        public void When_Saving_Current_Hands_Should_Set_Total_Cash_In_History_To_Current_Total_Cash()
        {
            _sut.CurrentHands.Add(new PlayerHand());
            _sut.SaveCurrentHands();

            Assert.AreEqual(TOTAL_CASH_AMOUNT, _sut.HandHistory.Last().TotalPlayerCashAfterOutcome);
        }

        [TestMethod]
        public void When_Current_Hands_Are_Cleared_Should_Be_No_Current_Hands()
        {
            _sut.CurrentHands.Add(new PlayerHand());
            _sut.ClearCurrentHands();

            Assert.IsFalse(_sut.CurrentHands.Any());
        }

        [TestMethod]
        public void When_Accessing_In_Play_Hand_After_Clearing_Hands_Should_Throw_Exception()
        {
            _sut.ClearCurrentHands();

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => _sut.InPlayHand);
        }

        private static Mock<IDealer> GetMockDealerWith(TableSettings tableSettings)
        {
            var mockDealer = new Mock<IDealer>();
            mockDealer.Setup(md => md.TableSettings).Returns(tableSettings);
            return mockDealer;
        }

        private void GivePlayerDoubleDownableHand(ICard visibleCard)
        {
            var playerHand = GetPlayerHandWithTwoOfTheSameCard();
            _mockPlayerStrategy.Setup(mpb => mpb.ShouldDoubleDown(playerHand, visibleCard)).Returns(true);
            _sut.CurrentHands.Add(playerHand);
        }

        private void GivePlayerSplittableHand(ICard visibleCard)
        {
            var playerHand = GetPlayerHandWithTwoOfTheSameCard();
            _mockPlayerStrategy.Setup(mpb => mpb.ShouldSplit(playerHand, visibleCard)).Returns(true);
            _sut.CurrentHands.Add(playerHand);
        }

        private void GivePlayerMockSplittableHand(ICard visibleCard, decimal betAmount)
        {
            var playerHand = GetSplittableMockPlayerHand(betAmount);
            _mockPlayerStrategy.Setup(mpb => mpb.ShouldSplit(playerHand, visibleCard)).Returns(true);
            _sut.CurrentHands.Add(playerHand);
        }

        private PlayerHand GetPlayerHandWithTwoOfTheSameCard()
        {
            var playerHand = new PlayerHand {Bet = TOTAL_CASH_AMOUNT / 10};
            playerHand.Cards.Add(new Card(CardType.Eight, CardSuit.Clubs, _blackjackCardValueAssigner));
            playerHand.Cards.Add(new Card(CardType.Eight, CardSuit.Clubs, _blackjackCardValueAssigner));
            return playerHand;
        }

        private IPlayerHand GetSplittableMockPlayerHand(decimal betAmount)
        {
            var mockPlayerHand = MockRepository.GenerateMock<IPlayerHand>();
            mockPlayerHand.Stub(mph => mph.Bet).Return(betAmount);
            var mockPlayerCards = new List<ICard>
            {
                new Card(CardType.Eight, CardSuit.Clubs, _blackjackCardValueAssigner),
                new Card(CardType.Eight, CardSuit.Clubs, _blackjackCardValueAssigner)
            };
            mockPlayerHand.Stub(mph => mph.Cards).Return(mockPlayerCards);
            mockPlayerHand.Stub(mph => mph.CanSplit()).Return(true);

            return mockPlayerHand;
        }
    }
}
