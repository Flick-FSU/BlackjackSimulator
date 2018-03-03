using System;
using BlackjackSimulator;
using BlackjackSimulator.Entities;
using BlackjackSimulator.Models;
using BlackjackSimulator.Strategies;
using GamblingLibrary;
using GamblingLibrary.Enums;
using GamblingLibrary.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlackjackSimulatorTest
{
    [TestClass]
    public class BasicMinimumPlayerStrategyTest
    {
        private readonly BasicMinimumPlayerStrategy _sut;
        private readonly TableSettings _tableSettings;
        private readonly ICardValueAssigner _blackjackCardValueAssigner;
        private readonly ICard _nullCard;
        private readonly ICard _aceCard;
        private readonly PlayerHand _currentHand;

        public BasicMinimumPlayerStrategyTest()
        {
            const int minimumBet = 10;
            const int maximumBet = 500;
            const int maximumPlayers = 4;
            _tableSettings = new TableSettings(minimumBet, maximumBet, maximumPlayers);
            _currentHand = new PlayerHand();
            _blackjackCardValueAssigner = new BlackjackCardValueAssigner();
            _nullCard = new NullCard();
            _aceCard = new Card(CardType.Ace, CardSuit.Clubs, _blackjackCardValueAssigner);
            _sut = new BasicMinimumPlayerStrategy();
        }

        [TestInitialize]
        public void MyTestInitialize()
        {
            _currentHand.Cards.Clear();
        }

        [TestMethod]
        public void When_Getting_Initial_Bet_Should_Equal_Minimum_Bet()
        {
            decimal cashTotal = 20;

            Assert.AreEqual(_tableSettings.MinimumBet, _sut.GetInitialBetAmount(_currentHand, cashTotal, _tableSettings));
        }

        [TestMethod]
        public void When_Getting_Initial_Bet_Should_Throw_Exception_If_Players_Cash_Total_Is_Under_Table_Minimum()
        {
            decimal cashTotal = _tableSettings.MinimumBet - 1;

            Assert.ThrowsException<InvalidOperationException>(() => _sut.GetInitialBetAmount(_currentHand, cashTotal, _tableSettings));
        }

        [TestMethod]
        public void When_Deciding_To_Split_Should_Return_False_If_Hand_Has_Cards_That_Are_Not_Splittable()
        {
            _currentHand.Cards.Add(_aceCard);
            _currentHand.Cards.Add(new Card(CardType.Eight, CardSuit.Clubs, _blackjackCardValueAssigner));

            Assert.IsFalse(_sut.ShouldSplit(_currentHand, _nullCard));
        }

        [TestMethod]
        public void When_Deciding_To_Split_Should_Return_False_If_Hand_Does_Not_Have_Exactly_Two_Cards()
        {
            _currentHand.Cards.Add(_aceCard);
            _currentHand.Cards.Add(_aceCard);
            _currentHand.Cards.Add(_aceCard);

            Assert.IsFalse(_sut.ShouldSplit(_currentHand, _nullCard));
        }

        [TestMethod]
        public void When_Deciding_To_Split_Should_Return_True_If_Hand_Has_Exactly_Two_Cards_That_Are_Aces()
        {
            _currentHand.Cards.Add(_aceCard);
            _currentHand.Cards.Add(_aceCard);

            Assert.IsTrue(_sut.ShouldSplit(_currentHand, _nullCard));
        }

        [TestMethod]
        public void When_Deciding_To_Split_Should_Return_False_If_Hand_Has_Exactly_Two_Of_The_Same_Cards_That_Are_Not_Aces()
        {
            _currentHand.Cards.Add(new Card(CardType.King, CardSuit.Clubs, _blackjackCardValueAssigner));
            _currentHand.Cards.Add(new Card(CardType.King, CardSuit.Diamonds, _blackjackCardValueAssigner));

            Assert.IsFalse(_sut.ShouldSplit(_currentHand, _nullCard));
        }

        [TestMethod]
        public void When_Deciding_To_Double_Down_Should_Return_True_If_Single_Hand_Value_Is_An_Eleven()
        {
            _currentHand.Cards.Add(new Card(CardType.Eight, CardSuit.Clubs, _blackjackCardValueAssigner));
            _currentHand.Cards.Add(new Card(CardType.Three, CardSuit.Clubs, _blackjackCardValueAssigner));

            Assert.IsTrue(_sut.ShouldDoubleDown(_currentHand, _nullCard));
        }

        [TestMethod]
        public void When_Deciding_To_Double_Down_Should_Return_False_If_Hand_Has_Values_Of_Eleven_And_Twenty_One()
        {
            _currentHand.Cards.Add(_aceCard);
            _currentHand.Cards.Add(new Card(CardType.Ten, CardSuit.Clubs, _blackjackCardValueAssigner));

            Assert.IsFalse(_sut.ShouldDoubleDown(_currentHand, _nullCard));
        }

        [TestMethod]
        public void When_Deciding_To_Double_Down_Should_Return_False_If_Hand_Has_No_Values_Of_Eleven()
        {
            _currentHand.Cards.Add(_aceCard);
            _currentHand.Cards.Add(new Card(CardType.Four, CardSuit.Clubs, _blackjackCardValueAssigner));

            Assert.IsFalse(_sut.ShouldDoubleDown(_currentHand, _nullCard));
        }

        [TestMethod]
        public void When_Deciding_To_Hit_Should_Return_True_If_Single_Hand_Value_Less_Than_Seventeen()
        {
            _currentHand.Cards.Add(new Card(CardType.Eight, CardSuit.Clubs, _blackjackCardValueAssigner));
            _currentHand.Cards.Add(new Card(CardType.Four, CardSuit.Clubs, _blackjackCardValueAssigner));

            Assert.IsTrue(_sut.ShouldHit(_currentHand, _nullCard));
        }

        [TestMethod]
        public void When_Deciding_To_Hit_Should_Return_True_If_Multiple_Hand_Values_All_Less_Than_Seventeen()
        {
            _currentHand.Cards.Add(_aceCard);
            _currentHand.Cards.Add(new Card(CardType.Four, CardSuit.Clubs, _blackjackCardValueAssigner));

            Assert.IsTrue(_sut.ShouldHit(_currentHand, _nullCard));
        }

        [TestMethod]
        public void When_Deciding_To_Hit_Should_Return_False_If_Hand_Values_Contain_Number_Over_Sixteen_And_Under_Twenty_Two()
        {
            _currentHand.Cards.Add(_aceCard);
            _currentHand.Cards.Add(new Card(CardType.Six, CardSuit.Clubs, _blackjackCardValueAssigner));

            Assert.IsFalse(_sut.ShouldHit(_currentHand, _nullCard));
        }

        [TestMethod]
        public void When_Deciding_To_Hit_Should_Return_False_If_All_Hand_Values_Are_Over_Twenty_One()
        {
            _currentHand.Cards.Add(_aceCard);
            var nineCard = new Card(CardType.Nine, CardSuit.Clubs, _blackjackCardValueAssigner);
            _currentHand.Cards.Add(nineCard);
            _currentHand.Cards.Add(nineCard);
            _currentHand.Cards.Add(nineCard);

            Assert.IsFalse(_sut.ShouldHit(_currentHand, _nullCard));
        }

        [TestMethod]
        public void When_Deciding_To_Leave_Table_Should_Return_True_If_Total_Cash_Is_Less_Than_Minimum_Bet()
        {
            Assert.IsTrue(_sut.ShouldLeaveTable(_tableSettings.MinimumBet - 1, _tableSettings));
        }

        [TestMethod]
        public void When_Deciding_To_Leave_Table_Should_Return_False_If_Total_Cash_Is_Greater_Than_Or_Equal_To_Minimum_Bet()
        {
            Assert.IsFalse(_sut.ShouldLeaveTable(_tableSettings.MinimumBet + 1, _tableSettings));
        }
    }
}
