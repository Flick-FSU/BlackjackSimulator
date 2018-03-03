using GamblingLibrary;
using GamblingLibrary.Enums;
using GamblingLibrary.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace GamblingLibraryTest
{
    [TestClass]
    public class CardTest
    {
        [TestMethod]
        public void When_Assigner_Allows_Should_Be_Able_To_Change_Card_Value()
        {
            const int oldAceValue = 1;
            const int newAceValue = 11;
            const CardType cardTypeToTest = CardType.Ace;
            const CardSuit cardSuitToTest = CardSuit.Clubs;
            var cardValueAssigner = MockRepository.GenerateMock<ICardValueAssigner>();
            cardValueAssigner.Stub(cva => cva.CanAssignNewValueFor(cardTypeToTest, cardSuitToTest, newAceValue)).Return(true);
            cardValueAssigner.Stub(cva => cva.GetCardValueFor(cardTypeToTest, CardSuit.Clubs)).Return(oldAceValue);

            var sut = new Card(cardTypeToTest, cardSuitToTest, cardValueAssigner);
            sut.OverrideValue(newAceValue);

            Assert.AreEqual(sut.Value, newAceValue);
        }

        [TestMethod]
        public void When_Assigner_Does_Not_Allow_Should_Not_Be_Able_To_Change_Card_Value()
        {
            const int oldJackValue = 10;
            const int newJackValue = 11;
            const CardType cardTypeToTest = CardType.Jack;
            const CardSuit cardSuitToTest = CardSuit.Clubs;
            var cardValueAssigner = MockRepository.GenerateMock<ICardValueAssigner>();
            cardValueAssigner.Stub(cva => cva.CanAssignNewValueFor(cardTypeToTest, cardSuitToTest, newJackValue)).Return(false);
            cardValueAssigner.Stub(cva => cva.GetCardValueFor(cardTypeToTest, CardSuit.Clubs)).Return(oldJackValue);

            var sut = new Card(cardTypeToTest, cardSuitToTest, cardValueAssigner);
            sut.OverrideValue(newJackValue);

            Assert.AreNotEqual(sut.Value, newJackValue);
        }
    }
}
