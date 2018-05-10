using GamblingLibrary;
using GamblingLibrary.Enums;
using GamblingLibrary.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace GamblingLibraryTest
{
    [TestClass]
    public class CardTest
    {
        [TestMethod]
        public void When_Assigner_Allows_Should_Be_Able_To_Change_Card_Value()
        {
            const int newAceValue = 11;
            const CardType cardTypeToTest = CardType.Ace;
            const CardSuit cardSuitToTest = CardSuit.Clubs;
            var cardValueAssigner = new Mock<ICardValueAssigner>();
            cardValueAssigner.Setup(cva => cva.CanAssignNewValueFor(cardTypeToTest, cardSuitToTest, newAceValue)).Returns(true);

            var sut = new Card(cardTypeToTest, cardSuitToTest, cardValueAssigner.Object);
            sut.OverrideValue(newAceValue);

            Assert.AreEqual(sut.Value, newAceValue);
        }

        [TestMethod]
        public void When_Assigner_Does_Not_Allow_Should_Not_Be_Able_To_Change_Card_Value()
        {
            const int newJackValue = 11;
            const CardType cardTypeToTest = CardType.Jack;
            const CardSuit cardSuitToTest = CardSuit.Clubs;
            var cardValueAssigner = new Mock<ICardValueAssigner>();
            cardValueAssigner.Setup(cva => cva.CanAssignNewValueFor(cardTypeToTest, cardSuitToTest, newJackValue)).Returns(false);

            var sut = new Card(cardTypeToTest, cardSuitToTest, cardValueAssigner.Object);
            sut.OverrideValue(newJackValue);

            Assert.AreNotEqual(sut.Value, newJackValue);
        }
    }
}
