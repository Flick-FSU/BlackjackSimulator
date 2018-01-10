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
        public void Should_Be_Able_To_Change_Value_When_Assigner_Allows()
        {
            const int newAceValue = 11;
            const CardType cardTypeToTest = CardType.Ace;
            const CardSuit cardSuitToTest = CardSuit.Clubs;
            var cardValueAssigner = MockRepository.GenerateMock<ICardValueAssigner>();
            cardValueAssigner.Stub(cva => cva.CanAssignValueFor(cardTypeToTest, cardSuitToTest, newAceValue)).Return(true);
            cardValueAssigner.Stub(cva => cva.GetCardValueFor(cardTypeToTest, CardSuit.Clubs)).Return(1);

            var card = new Card(cardTypeToTest, cardSuitToTest, cardValueAssigner);
            card.OverrideValue(newAceValue);

            Assert.AreEqual(card.Value, newAceValue);
        }

        [TestMethod]
        public void Should_Not_Be_Able_To_Change_Value_When_Assigner_Rejects()
        {
            const int newJackValue = 11;
            const CardType cardTypeToTest = CardType.Jack;
            const CardSuit cardSuitToTest = CardSuit.Clubs;
            var cardValueAssigner = MockRepository.GenerateMock<ICardValueAssigner>();
            cardValueAssigner.Stub(cva => cva.CanAssignValueFor(cardTypeToTest, cardSuitToTest, newJackValue)).Return(false);
            cardValueAssigner.Stub(cva => cva.GetCardValueFor(cardTypeToTest, CardSuit.Clubs)).Return(10);

            var card = new Card(cardTypeToTest, cardSuitToTest, cardValueAssigner);
            card.OverrideValue(newJackValue);

            Assert.AreNotEqual(card.Value, newJackValue);
        }
    }
}
