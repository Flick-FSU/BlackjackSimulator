using System;
using System.Collections.Generic;
using BlackjackSimulator;
using BlackjackSimulator.Entities;
using BlackjackSimulator.Entities.Interfaces;
using BlackjackSimulator.Models;
using BlackjackSimulator.Strategies;
using BlackjackSimulator.Strategies.Interfaces;
using GamblingLibrary;
using GamblingLibrary.Enums;
using GamblingLibrary.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Rhino.Mocks;

namespace BlackjackSimulatorTest
{
    [TestClass]
    public class DealerTest
    {
        private const int MAX_NUMBER_OF_PLAYERS = 1;
        private Mock<IGroupOfCards> _mockGroupOfCards;
        private Mock<IGameManager> _mockGameManager;
        private IDealer _sut;
        private readonly ICard _nullCard;
        private readonly TableSettings _tableSettings;
        private readonly IDealerStrategy _dealerStrategy;

        public DealerTest()
        {
            _nullCard = new NullCard();
            _tableSettings = new TableSettings(10, 100, MAX_NUMBER_OF_PLAYERS);
            _dealerStrategy = new StandardDealerStrategy();
        }

        [TestInitialize]
        public void MyTestInitialize()
        {
            _mockGroupOfCards = new Mock<IGroupOfCards>();
            _mockGroupOfCards.Setup(mgoc => mgoc.Cards).Returns(new List<ICard>());
            _mockGameManager = new Mock<IGameManager>();
            _sut = new Dealer(_mockGroupOfCards.Object, _mockGameManager.Object, _dealerStrategy);
            _sut.SetTableSettingsWith(_tableSettings);
        }

        [TestMethod]
        public void When_Registering_A_Player_Should_Add_One_To_Registered_Players_List()
        {
            var mockBlackjackPlayer = new Mock<IPlayer>();

            _sut.Register(mockBlackjackPlayer.Object);
            Assert.AreEqual(1, _sut.RegisteredPlayers.Count);
        }

        [TestMethod]
        public void When_A_Player_Is_Registered_Should_Be_Able_To_Unregister_It()
        {
            var mockBlackjackPlayer = new Mock<IPlayer>();

            _sut.Register(mockBlackjackPlayer.Object);
            _sut.Unregister(mockBlackjackPlayer.Object);
            Assert.AreEqual(0, _sut.RegisteredPlayers.Count);
        }

        [TestMethod]
        public void When_Starting_Game_With_No_Players_Should_Throw_Exception()
        {
            Assert.ThrowsException<InvalidOperationException>(() => _sut.PlaySingleGame());
        }

        [TestMethod]
        public void When_Starting_Game_Should_Shuffle_Cards()
        {
            _sut.Register(GetMockBlackjackPlayerWithMockHand().Object);
            _mockGameManager.Setup(mgm => mgm.CollectCardsFrom(_sut.RegisteredPlayers)).Returns(new List<ICard>());
            _sut.CurrentCards.Add(_nullCard);
            _sut.CurrentCards.Add(_nullCard);
            _mockGameManager.Setup(mgm => mgm.CollectCardsFrom(_sut.CurrentCards)).Returns(new List<ICard>());
            _sut.PlaySingleGame();

            _mockGroupOfCards.Verify(c => c.Shuffle());
        }

        [TestMethod]
        public void When_Finished_Initial_Deal_Dealer_Should_Have_A_Card_Visible()
        {
            _mockGroupOfCards.Setup(mgoc => mgoc.PullTopCard())
                .Returns(new Card(CardType.Eight, CardSuit.Clubs, new BlackjackCardValueAssigner()));
            var mockBlackjackPlayer = GetMockBlackjackPlayerWithMockHand();
            _sut.Register(mockBlackjackPlayer.Object);
            _mockGameManager.Setup(mgm => mgm.CollectCardsFrom(It.IsAny<List<IPlayer>>())).Returns(new List<ICard>());
            _mockGameManager.Setup(mgm => mgm.CollectCardsFrom(It.IsAny<List<ICard>>())).Returns(new List<ICard>());
            var concreteGameManager = new GameManager();
            _mockGameManager.Setup(mgm => mgm.PlaceYourBets(_sut.RegisteredPlayers))
                .Callback(() => concreteGameManager.PlaceYourBets(_sut.RegisteredPlayers));
            _mockGameManager.Setup(mgm => mgm.DealInitialCards(_mockGroupOfCards.Object, _sut.RegisteredPlayers, _sut.CurrentCards))
                .Callback(() => concreteGameManager.DealInitialCards(_mockGroupOfCards.Object, _sut.RegisteredPlayers, _sut.CurrentCards));

            _sut.PlaySingleGame();

            Assert.IsNotNull(_sut.VisibleCard);
        }

        [TestMethod]
        public void When_Single_Game_Is_Played_Without_Dealer_Blackjack_Should_Call_Game_Steps_In_Proper_Order()
        {
            var strictMockGameManager = new Mock<IGameManager>(MockBehavior.Strict);
            var sut = new Dealer(_mockGroupOfCards.Object, strictMockGameManager.Object, _dealerStrategy);
            sut.SetTableSettingsWith(_tableSettings);
            sut.CurrentCards.Add(_nullCard);
            sut.CurrentCards.Add(_nullCard);
            sut.Register(GetMockBlackjackPlayerWithMockHand().Object);

            strictMockGameManager.Setup(mgm => mgm.PlaceYourBets(sut.RegisteredPlayers));
            strictMockGameManager.Setup(mgm => mgm.DealInitialCards(It.IsAny<IGroupOfCards>(), It.IsAny<List<IPlayer>>(), It.IsAny<List<ICard>>()));
            strictMockGameManager.Setup(mgm => mgm.PlayersPlay(It.IsAny<IGroupOfCards>(), It.IsAny<List<IPlayer>>(), It.IsAny<ICard>()));
            strictMockGameManager.Setup(mgm => mgm.DealerPlays(It.IsAny<IGroupOfCards>(), It.IsAny<List<ICard>>(), It.IsAny<IDealerStrategy>()));
            strictMockGameManager.Setup(mgm => mgm.DeterminePlayerHandOutcomes(It.IsAny<List<ICard>>(), It.IsAny<List<IPlayer>>()));
            strictMockGameManager.Setup(mgm => mgm.PayoutOrCollectBetsFrom(It.IsAny<List<IPlayer>>())).Returns(0);
            strictMockGameManager.Setup(mgm => mgm.SaveCurrentHandResultsOf(It.IsAny<List<IPlayer>>()));
            strictMockGameManager.Setup(mgm => mgm.CollectCardsFrom(It.IsAny<List<IPlayer>>())).Returns(new List<ICard>());
            strictMockGameManager.Setup(mgm => mgm.CollectCardsFrom(It.IsAny<List<ICard>>())).Returns(new List<ICard>());
            strictMockGameManager.Setup(mgm => mgm.ClearHandsFrom(It.IsAny<List<IPlayer>>()));
            strictMockGameManager.Setup(mgm => mgm.DetermineToLeaveGameOrStay(It.IsAny<List<IPlayer>>()));

            sut.PlaySingleGame();
            strictMockGameManager.VerifyAll();
        }

        [TestMethod]
        public void When_Single_Game_Is_Played_With_Dealer_Blackjack_Should_Skip_Player_Plays_And_Dealer_Plays_Game_Steps()
        {
            var strictMockGameManager = new Mock<IGameManager>(MockBehavior.Strict);
            var sut = new Dealer(_mockGroupOfCards.Object, strictMockGameManager.Object, _dealerStrategy);
            sut.SetTableSettingsWith(_tableSettings);
            var blackjackCardValueAssigner = new BlackjackCardValueAssigner();
            sut.CurrentCards.Add(new Card(CardType.Ace, CardSuit.Clubs, blackjackCardValueAssigner));
            sut.CurrentCards.Add(new Card(CardType.Ten, CardSuit.Clubs, blackjackCardValueAssigner));
            sut.Register(GetMockBlackjackPlayerWithMockHand().Object);

            strictMockGameManager.Setup(mgm => mgm.PlaceYourBets(sut.RegisteredPlayers));
            strictMockGameManager.Setup(mgm => mgm.DealInitialCards(It.IsAny<IGroupOfCards>(), It.IsAny<List<IPlayer>>(), It.IsAny<List<ICard>>()));
            strictMockGameManager.Setup(mgm => mgm.DeterminePlayerHandOutcomes(It.IsAny<List<ICard>>(), It.IsAny<List<IPlayer>>()));
            strictMockGameManager.Setup(mgm => mgm.PayoutOrCollectBetsFrom(It.IsAny<List<IPlayer>>())).Returns(0);
            strictMockGameManager.Setup(mgm => mgm.SaveCurrentHandResultsOf(It.IsAny<List<IPlayer>>()));
            strictMockGameManager.Setup(mgm => mgm.CollectCardsFrom(It.IsAny<List<IPlayer>>())).Returns(new List<ICard>());
            strictMockGameManager.Setup(mgm => mgm.CollectCardsFrom(It.IsAny<List<ICard>>())).Returns(new List<ICard>());
            strictMockGameManager.Setup(mgm => mgm.ClearHandsFrom(It.IsAny<List<IPlayer>>()));
            strictMockGameManager.Setup(mgm => mgm.DetermineToLeaveGameOrStay(It.IsAny<List<IPlayer>>()));

            sut.PlaySingleGame();
            strictMockGameManager.VerifyAll();
        }

        [TestMethod]
        public void When_Single_Game_Is_Played_Should_Add_Player_Hand_Cards_And_Dealer_Cards_Back_To_Deck()
        {
            var mockGameManager = new Mock<IGameManager>();
            var sut = new Dealer(_mockGroupOfCards.Object, mockGameManager.Object, _dealerStrategy);
            sut.SetTableSettingsWith(_tableSettings);
            var dealerCards = new List<ICard> { _nullCard, _nullCard };
            sut.CurrentCards.AddRange(dealerCards);
            sut.Register(GetMockBlackjackPlayerWithMockHand().Object);

            List<ICard> cardsToReturn = new List<ICard> { new NullCard() };
            mockGameManager.Setup(mgm => mgm.CollectCardsFrom(sut.RegisteredPlayers)).Returns(cardsToReturn);
            mockGameManager.Setup(mgm => mgm.CollectCardsFrom(sut.CurrentCards)).Returns(dealerCards);

            _mockGroupOfCards.Setup(mgoc => mgoc.Cards.AddRange(cardsToReturn));
            _mockGroupOfCards.Setup(mgoc => mgoc.Cards.AddRange(dealerCards));

            sut.PlaySingleGame();
            
            _mockGroupOfCards.VerifyAll();
        }

        private Mock<IPlayer> GetMockBlackjackPlayerWithMockHand()
        {
            var hands = new List<IPlayerHand>();
            var blackjackPlayer = new Mock<IPlayer>();
            blackjackPlayer.Setup(bp => bp.CurrentHands).Returns(hands);

            return blackjackPlayer;
        }
    }
}
