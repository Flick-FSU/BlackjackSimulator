using System;
using System.Collections.Generic;
using System.Linq;
using BlackjackSimulator;
using BlackjackSimulator.Entities;
using BlackjackSimulator.Entities.Interfaces;
using BlackjackSimulator.Enums;
using GamblingLibrary;
using GamblingLibrary.Enums;
using GamblingLibrary.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlackjackSimulatorTest
{
    [TestClass]
    public class PlayerHandTest
    {
        private PlayerHand _sut;
        private readonly BlackjackCardValueAssigner _blackjackCardValueAssigner;

        public PlayerHandTest()
        {
            _blackjackCardValueAssigner = new BlackjackCardValueAssigner();
        }

        [TestInitialize]
        public void MyTestInitialize()
        {
            _sut = new PlayerHand();
        }

        [TestMethod]
        public void When_Card_Count_Not_Equal_To_Two_Should_Say_Cannot_Split()
        {
            _sut.Cards.Add(new NullCard());

            Assert.IsFalse(_sut.CanSplit());
        }

        [TestMethod]
        public void When_Cards_Have_Two_Unequal_Types_Should_Say_Cannot_Split()
        {
            _sut.Cards.Add(new Card(CardType.Ace, CardSuit.Clubs, _blackjackCardValueAssigner));
            _sut.Cards.Add(new Card(CardType.Eight, CardSuit.Clubs, _blackjackCardValueAssigner));

            Assert.IsFalse(_sut.CanSplit());
        }

        [TestMethod]
        public void When_Cards_Have_Two_Equal_Types_And_Only_Two_Cards_Should_Say_Can_Split()
        {
            _sut.Cards.AddRange(GetSplittableCards());

            Assert.IsTrue(_sut.CanSplit());
        }

        [TestMethod]
        public void When_Cards_Have_Two_Equal_Types_And_More_Than_Two_Cards_Should_Say_Cannot_Split()
        {
            _sut.Cards.AddRange(GetSplittableCards());
            _sut.Cards.Add(new Card(CardType.Eight, CardSuit.Clubs, _blackjackCardValueAssigner));

            Assert.IsFalse(_sut.CanSplit());
        }

        [TestMethod]
        public void When_Trying_To_Split_Should_Throw_Exception_If_Cards_Are_Not_Splittable()
        {
            _sut.Cards.Add(new Card(CardType.Ace, CardSuit.Clubs, _blackjackCardValueAssigner));
            _sut.Cards.Add(new Card(CardType.Eight, CardSuit.Clubs, _blackjackCardValueAssigner));

            Assert.ThrowsException<InvalidOperationException>(() => _sut.Split());
        }

        [TestMethod]
        public void When_Splitting_Splittable_Cards_Should_Return_Hand_With_Bet_Equal_To_This_Hand()
        {
            _sut.Cards.AddRange(GetSplittableCards());

            var newSplitHand = _sut.Split();
            Assert.AreEqual(_sut.Bet, newSplitHand.Bet);
        }

        [TestMethod]
        public void When_Splitting_Splittable_Cards_Should_Remove_One_Card_From_This_Hand()
        {
            _sut.Cards.AddRange(GetSplittableCards());

            _sut.Split();
            Assert.AreEqual(1, _sut.Cards.Count);
        }

        [TestMethod]
        public void When_Splitting_Splittable_Cards_Should_Return_Hand_With_One_Card()
        {
            _sut.Cards.AddRange(GetSplittableCards());

            var newSplitHand = _sut.Split();
            Assert.AreEqual(1, newSplitHand.Cards.Count);
        }

        [TestMethod]
        public void When_Splitting_Splittable_Cards_Should_Return_Hand_With_In_Play_Status()
        {
            _sut.Cards.AddRange(GetSplittableCards());

            var newSplitHand = _sut.Split();
            Assert.AreEqual(HandOutcome.InProgress, newSplitHand.Outcome);
        }

        [TestMethod]
        public void When_Splitting_Splittable_Cards_Should_Return_Hand_With_Card_Type_Equal_To_This_Hand_Card_Type()
        {
            _sut.Cards.AddRange(GetSplittableCards());

            var newSplitHand = _sut.Split();
            Assert.AreEqual(_sut.Cards.First().Type, newSplitHand.Cards.First().Type);
        }

        [TestMethod]
        public void When_Splitting_Splittable_Cards_Should_Set_Split_Flag()
        {
            _sut.Cards.AddRange(GetSplittableCards());

            _sut.Split();
            Assert.IsTrue(_sut.IsASplit);
        }

        [TestMethod]
        public void When_Splitting_Splittable_Cards_New_Hand_Should_Set_Split_Flag()
        {
            _sut.Cards.AddRange(GetSplittableCards());

            var newPlayerHand = _sut.Split();
            Assert.IsTrue(newPlayerHand.IsASplit);
        }

        [TestMethod]
        public void When_Getting_Deep_Copy_Of_Player_Hand_Should_Have_No_References_To_The_Same_Object()
        {
            _sut.Cards.Add(new Card(CardType.Eight, CardSuit.Clubs, _blackjackCardValueAssigner));
            _sut.Cards.Add(new Card(CardType.Jack, CardSuit.Clubs, _blackjackCardValueAssigner));
            _sut.Bet = 10M;

            IPlayerHand deepCopyOfPlayerHand = _sut.GetDeepCopy();
            Assert.AreNotSame(_sut, deepCopyOfPlayerHand);
            Assert.AreNotSame(_sut.Cards, deepCopyOfPlayerHand.Cards);
            for (int cardIndex = 0; cardIndex < _sut.Cards.Count; cardIndex++)
                Assert.AreNotSame(_sut.Cards.ElementAt(cardIndex), deepCopyOfPlayerHand.Cards.ElementAt(cardIndex));
        }

        [TestMethod]
        public void When_Getting_Deep_Copy_Of_Player_Hand_Should_Have_Same_Card_Values_In_The_Copy()
        {
            _sut.Cards.Add(new Card(CardType.Eight, CardSuit.Clubs, _blackjackCardValueAssigner));
            _sut.Cards.Add(new Card(CardType.Jack, CardSuit.Clubs, _blackjackCardValueAssigner));
            _sut.Bet = 10M;

            IPlayerHand deepCopyOfPlayerHand = _sut.GetDeepCopy();
            for (int cardIndex = 0; cardIndex < _sut.Cards.Count; cardIndex++)
                Assert.AreEqual(_sut.Cards.ElementAt(cardIndex), deepCopyOfPlayerHand.Cards.ElementAt(cardIndex));
        }

        [TestMethod]
        public void When_Checking_If_Hand_Is_Blackjack_Should_Return_False_If_Hand_Is_A_Split()
        {
            _sut.Cards.Add(new Card(CardType.Ace, CardSuit.Clubs, _blackjackCardValueAssigner));
            _sut.Cards.Add(new Card(CardType.Ace, CardSuit.Clubs, _blackjackCardValueAssigner));
            _sut.Split();
            _sut.Cards.Add(new Card(CardType.Ten, CardSuit.Clubs, _blackjackCardValueAssigner));

            Assert.IsFalse(_sut.IsBlackjack);
        }

        private List<ICard> GetSplittableCards()
        {
            return new List<ICard>
            {
                new Card(CardType.Ace, CardSuit.Clubs, _blackjackCardValueAssigner),
                new Card(CardType.Ace, CardSuit.Clubs, _blackjackCardValueAssigner)
            };
        }
    }
}
