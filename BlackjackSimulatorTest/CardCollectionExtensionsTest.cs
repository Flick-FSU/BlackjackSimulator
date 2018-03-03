using System.Collections.Generic;
using System.Linq;
using BlackjackSimulator;
using BlackjackSimulator.Extensions;
using GamblingLibrary;
using GamblingLibrary.Enums;
using GamblingLibrary.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlackjackSimulatorTest
{
    [TestClass]
    public class CardCollectionExtensionsTest
    {
        private List<ICard> _sut;
        private readonly BlackjackCardValueAssigner _blackjackCardValueAssigner;

        public CardCollectionExtensionsTest()
        {
            _blackjackCardValueAssigner = new BlackjackCardValueAssigner();
        }

        [TestInitialize]
        public void MyTestInitialize()
        {
            _sut = new List<ICard>();
        }

        [TestMethod]
        public void When_Getting_Values_For_No_Cards_Should_Return_Single_Value_Of_Zero()
        {
            Assert.AreEqual(0, _sut.GetCardValues().Last());
        }

        [TestMethod]
        public void When_Getting_Values_For_Two_Non_Ace_Cards_Should_Return_One_Value()
        {
            _sut.Add(new Card(CardType.Five, CardSuit.Clubs, _blackjackCardValueAssigner));
            _sut.Add(new Card(CardType.Eight, CardSuit.Clubs, _blackjackCardValueAssigner));

            Assert.AreEqual(1, _sut.GetCardValues().Count);
        }

        [TestMethod]
        public void When_Getting_Values_For_One_Non_Ace_Card_And_One_Ace_Card_Should_Return_Two_Values()
        {
            _sut.Add(new Card(CardType.Ace, CardSuit.Clubs, _blackjackCardValueAssigner));
            _sut.Add(new Card(CardType.Eight, CardSuit.Clubs, _blackjackCardValueAssigner));

            Assert.AreEqual(2, _sut.GetCardValues().Count);
        }

        [TestMethod]
        public void When_Getting_Values_For_Two_Ace_Cards_Should_Return_Three_Values()
        {
            _sut.Add(new Card(CardType.Ace, CardSuit.Clubs, _blackjackCardValueAssigner));
            _sut.Add(new Card(CardType.Ace, CardSuit.Clubs, _blackjackCardValueAssigner));

            Assert.AreEqual(3, _sut.GetCardValues().Count);
        }

        [TestMethod]
        public void When_Getting_Values_For_Three_Ace_Cards_Should_Return_Four_Values()
        {
            _sut.Add(new Card(CardType.Ace, CardSuit.Clubs, _blackjackCardValueAssigner));
            _sut.Add(new Card(CardType.Ace, CardSuit.Clubs, _blackjackCardValueAssigner));
            _sut.Add(new Card(CardType.Ace, CardSuit.Clubs, _blackjackCardValueAssigner));

            Assert.AreEqual(4, _sut.GetCardValues().Count);
        }

        [TestMethod]
        public void When_Getting_Values_For_Four_Ace_Cards_Should_Return_Five_Values()
        {
            _sut.Add(new Card(CardType.Ace, CardSuit.Clubs, _blackjackCardValueAssigner));
            _sut.Add(new Card(CardType.Ace, CardSuit.Clubs, _blackjackCardValueAssigner));
            _sut.Add(new Card(CardType.Ace, CardSuit.Clubs, _blackjackCardValueAssigner));
            _sut.Add(new Card(CardType.Ace, CardSuit.Clubs, _blackjackCardValueAssigner));

            Assert.AreEqual(5, _sut.GetCardValues().Count);
        }

        [TestMethod]
        public void When_Getting_Values_For_Two_Non_Ace_Cards_Should_Return_One_Value_Of_Cards_Added_Together()
        {
            _sut.Add(new Card(CardType.Five, CardSuit.Clubs, _blackjackCardValueAssigner));
            _sut.Add(new Card(CardType.Eight, CardSuit.Clubs, _blackjackCardValueAssigner));

            Assert.AreEqual(13, _sut.GetCardValues().Last());
        }

        [TestMethod]
        public void
            When_Getting_Best_Value_For_Non_Busted_Cards_Should_Return_Highest_Value_Between_Seventeen_And_Twenty_One()
        {
            _sut.Add(new Card(CardType.Ace, CardSuit.Clubs, _blackjackCardValueAssigner));
            _sut.Add(new Card(CardType.Ace, CardSuit.Clubs, _blackjackCardValueAssigner));
            _sut.Add(new Card(CardType.Six, CardSuit.Clubs, _blackjackCardValueAssigner));

            Assert.AreEqual(18, _sut.GetBestCardValue());
        }

        [TestMethod]
        public void
            When_Getting_Best_Value_For_Busted_Cards_Should_Return_Lowest_Value()
        {
            _sut.Add(new Card(CardType.Ace, CardSuit.Clubs, _blackjackCardValueAssigner));
            _sut.Add(new Card(CardType.Ace, CardSuit.Clubs, _blackjackCardValueAssigner));
            _sut.Add(new Card(CardType.Ten, CardSuit.Clubs, _blackjackCardValueAssigner));
            _sut.Add(new Card(CardType.Ten, CardSuit.Clubs, _blackjackCardValueAssigner));

            Assert.AreEqual(22, _sut.GetBestCardValue());
        }

        [TestMethod]
        public void When_Checking_If_Two_Cards_Of_Twenty_One_Total_Value_Is_Blackjack_Should_Return_True()
        {
            _sut.Add(new Card(CardType.Ace, CardSuit.Clubs, _blackjackCardValueAssigner));
            _sut.Add(new Card(CardType.Jack, CardSuit.Clubs, _blackjackCardValueAssigner));

            Assert.IsTrue(_sut.IsBlackjack());
        }

        [TestMethod]
        public void When_Checking_If_Two_Cards_Of_Non_Twenty_One_Total_Value_Is_Blackjack_Should_Return_False()
        {
            _sut.Add(new Card(CardType.Five, CardSuit.Clubs, _blackjackCardValueAssigner));
            _sut.Add(new Card(CardType.Jack, CardSuit.Clubs, _blackjackCardValueAssigner));

            Assert.IsFalse(_sut.IsBlackjack());
        }

        [TestMethod]
        public void When_Checking_If_Three_Cards_Of_Twenty_One_Total_Value_Is_Blackjack_Should_Return_False()
        {
            _sut.Add(new Card(CardType.Eight, CardSuit.Clubs, _blackjackCardValueAssigner));
            _sut.Add(new Card(CardType.Jack, CardSuit.Clubs, _blackjackCardValueAssigner));
            _sut.Add(new Card(CardType.Three, CardSuit.Clubs, _blackjackCardValueAssigner));

            Assert.IsFalse(_sut.IsBlackjack());
        }
    }
}
