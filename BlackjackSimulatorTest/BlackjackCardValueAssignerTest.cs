using BlackjackSimulator;
using BlackjackSimulator.Entities;
using GamblingLibrary.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlackjackSimulatorTest
{
    [TestClass]
    public class BlackjackCardValueAssignerTest
    {
        private const int MINIMUM_CARD_SUIT = (int)CardSuit.Diamonds;
        private const int MAXIMUM_CARD_SUIT = (int)CardSuit.Spades;
        private readonly BlackjackCardValueAssigner _sut;

        public BlackjackCardValueAssignerTest()
        {
            _sut = new BlackjackCardValueAssigner();
        }

        [TestMethod]
        public void When_Getting_Values_For_Numbered_Cards_Should_Return_The_Same_Numeric_Value()
        {
            for (int cardIndex = (int) CardType.Two; cardIndex < (int) CardType.Jack; cardIndex++)
            {
                for (int suitIndex = MINIMUM_CARD_SUIT; suitIndex <= MAXIMUM_CARD_SUIT; suitIndex++)
                    Assert.AreEqual(cardIndex + 2,
                        _sut.GetCardValueFor((CardType) cardIndex, (CardSuit) suitIndex));
            }
        }

        [TestMethod]
        public void When_Getting_Values_For_Face_Cards_Should_Return_Value_Of_Ten()
        {
            for (int cardIndex = (int) CardType.Jack; cardIndex < (int) CardType.Ace; cardIndex++)
            {
                for (int suitIndex = MINIMUM_CARD_SUIT; suitIndex <= MAXIMUM_CARD_SUIT; suitIndex++)
                    Assert.AreEqual(10, _sut.GetCardValueFor((CardType) cardIndex, (CardSuit) suitIndex));
            }
        }

        [TestMethod]
        public void When_Getting_Values_For_Aces_Should_Return_Initial_Value_Of_Eleven()
        {
            for (int suitIndex = MINIMUM_CARD_SUIT; suitIndex <= MAXIMUM_CARD_SUIT; suitIndex++)
                Assert.AreEqual(11, _sut.GetCardValueFor(CardType.Ace, (CardSuit) suitIndex));
        }

        [TestMethod]
        public void When_Checking_If_Non_Ace_Cards_Can_Be_Assigned_A_New_Value_Should_Return_False()
        {
            for (int cardIndex = (int) CardType.Two; cardIndex < (int) CardType.Ace; cardIndex++)
            {
                for (int suitIndex = MINIMUM_CARD_SUIT; suitIndex <= MAXIMUM_CARD_SUIT; suitIndex++)
                    Assert.AreEqual(false, _sut.CanAssignNewValueFor((CardType) cardIndex, (CardSuit) suitIndex, 12));
            }
        }

        [TestMethod]
        public void When_Checking_If_Ace_Cards_Can_Be_Assigned_A_New_Value_Of_Eleven_Should_Return_True()
        {
            for (int suitIndex = MINIMUM_CARD_SUIT; suitIndex <= MAXIMUM_CARD_SUIT; suitIndex++)
                Assert.AreEqual(true, _sut.CanAssignNewValueFor(CardType.Ace, (CardSuit) suitIndex, 11));
        }

        [TestMethod]
        public void When_Checking_If_Ace_Cards_Can_Be_Assigned_A_New_Value_Of_One_Should_Return_True()
        {
            for (int suitIndex = MINIMUM_CARD_SUIT; suitIndex <= MAXIMUM_CARD_SUIT; suitIndex++)
                Assert.AreEqual(true, _sut.CanAssignNewValueFor(CardType.Ace, (CardSuit)suitIndex, 1));
        }

        [TestMethod]
        public void When_Checking_If_Ace_Cards_Can_Be_Assigned_A_New_Value_Of_Five_Should_Return_False()
        {
            for (int suitIndex = MINIMUM_CARD_SUIT; suitIndex <= MAXIMUM_CARD_SUIT; suitIndex++)
                Assert.AreEqual(false, _sut.CanAssignNewValueFor(CardType.Ace, (CardSuit)suitIndex, 5));
        }
    }
}
