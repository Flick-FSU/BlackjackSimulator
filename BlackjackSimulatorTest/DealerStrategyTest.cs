using System;
using System.Collections.Generic;
using BlackjackSimulator;
using BlackjackSimulator.Strategies;
using GamblingLibrary;
using GamblingLibrary.Enums;
using GamblingLibrary.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlackjackSimulatorTest
{
    [TestClass]
    public class DealerStrategyTest
    {
        private readonly StandardDealerStrategy _sut;
        private readonly BlackjackCardValueAssigner _blackjackCardValueAssigner;
        
        public DealerStrategyTest()
        {
            _sut = new StandardDealerStrategy();
            _blackjackCardValueAssigner = new BlackjackCardValueAssigner();
        }

        [TestMethod]
        public void When_Deciding_To_Hit_Should_Return_True_If_Single_Card_Value_Less_Than_Seventeen()
        {
            var dealerCards = new List<ICard>
            {
                new Card(CardType.Eight, CardSuit.Clubs, _blackjackCardValueAssigner),
                new Card(CardType.Eight, CardSuit.Clubs, _blackjackCardValueAssigner)
            };

            Assert.IsTrue(_sut.ShouldHit(dealerCards));
        }

        [TestMethod]
        public void When_Deciding_To_Hit_Should_Return_True_If_Multiple_Card_Values_All_Less_Than_Seventeen()
        {
            var dealerCards = new List<ICard>
            {
                new Card(CardType.Ace, CardSuit.Clubs, _blackjackCardValueAssigner),
                new Card(CardType.Five, CardSuit.Clubs, _blackjackCardValueAssigner)
            };

            Assert.IsTrue(_sut.ShouldHit(dealerCards));
        }

        [TestMethod]
        public void When_Deciding_To_Hit_Should_Return_False_If_Single_Card_Value_Between_Seventeen_And_Twenty_One()
        {
            var dealerCards = new List<ICard>
            {
                new Card(CardType.Jack, CardSuit.Clubs, _blackjackCardValueAssigner),
                new Card(CardType.Seven, CardSuit.Clubs, _blackjackCardValueAssigner)
            };

            Assert.IsFalse(_sut.ShouldHit(dealerCards));
        }

        [TestMethod]
        public void
            When_Deciding_To_Hit_Should_Return_False_If_Any_Of_Multiple_Card_Values_Between_Seventeen_And_Twenty_One()
        {
            var dealerCards = new List<ICard>
            {
                new Card(CardType.Ace, CardSuit.Clubs, _blackjackCardValueAssigner),
                new Card(CardType.Five, CardSuit.Clubs, _blackjackCardValueAssigner),
                new Card(CardType.Ace, CardSuit.Clubs, _blackjackCardValueAssigner)
            };

            Assert.IsFalse(_sut.ShouldHit(dealerCards));
        }

        [TestMethod]
        public void When_Deciding_To_Hit_Should_Return_False_If_All_Hand_Values_Greater_Than_Twenty_One()
        {
            var dealerCards = new List<ICard>
            {
                new Card(CardType.Ace, CardSuit.Clubs, _blackjackCardValueAssigner),
                new Card(CardType.Ten, CardSuit.Clubs, _blackjackCardValueAssigner),
                new Card(CardType.Ten, CardSuit.Clubs, _blackjackCardValueAssigner),
                new Card(CardType.Ace, CardSuit.Clubs, _blackjackCardValueAssigner)
            };

            Assert.IsFalse(_sut.ShouldHit(dealerCards));
        }
    }
}
