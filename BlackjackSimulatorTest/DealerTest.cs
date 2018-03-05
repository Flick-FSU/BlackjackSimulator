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
using Rhino.Mocks;

namespace BlackjackSimulatorTest
{
    [TestClass]
    public class DealerTest
    {
        private const int MAX_NUMBER_OF_PLAYERS = 1;
        private IGroupOfCards _mockGroupOfCards;
        private IGameManager _mockGameManager;
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
            _mockGroupOfCards = MockRepository.GenerateMock<IGroupOfCards>();
            _mockGroupOfCards.Stub(mgoc => mgoc.Cards).Return(new List<ICard>());
            _mockGameManager = MockRepository.GenerateMock<IGameManager>();
            _sut = new Dealer(_mockGroupOfCards, _mockGameManager, _dealerStrategy);
            _sut.SetTableSettingsWith(_tableSettings);
        }

        [TestMethod]
        public void When_Registering_A_Player_Should_Add_One_To_Registered_Players_List()
        {
            var mockBlackjackPlayer = MockRepository.GenerateMock<IPlayer>();

            _sut.Register(mockBlackjackPlayer);
            Assert.AreEqual(1, _sut.RegisteredPlayers.Count);
        }

        [TestMethod]
        public void When_A_Player_Is_Registered_Should_Be_Able_To_Unregister_It()
        {
            var mockBlackjackPlayer = MockRepository.GenerateMock<IPlayer>();

            _sut.Register(mockBlackjackPlayer);
            _sut.Unregister(mockBlackjackPlayer);
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
            _sut.Register(GetMockBlackjackPlayerWithMockHand());
            _mockGameManager.Stub(mgm => mgm.CollectCardsFrom(_sut.RegisteredPlayers)).Return(new List<ICard>());
            _sut.CurrentCards.Add(_nullCard);
            _sut.CurrentCards.Add(_nullCard);
            _mockGameManager.Stub(mgm => mgm.CollectCardsFrom(_sut.CurrentCards)).Return(new List<ICard>());
            _sut.PlaySingleGame();

            _mockGroupOfCards.AssertWasCalled(c => c.Shuffle());
        }

        [TestMethod]
        public void When_Finished_Initial_Deal_Dealer_Should_Have_A_Card_Visible()
        {
            _mockGroupOfCards.Stub(mgoc => mgoc.PullTopCard())
                .Return(new Card(CardType.Eight, CardSuit.Clubs, new BlackjackCardValueAssigner()));
            var mockBlackjackPlayer = GetMockBlackjackPlayerWithMockHand();
            _sut.Register(mockBlackjackPlayer);
            _mockGameManager.Stub(mgm => mgm.CollectCardsFrom(_sut.RegisteredPlayers)).IgnoreArguments().Return(new List<ICard>());
            _mockGameManager.Stub(mgm => mgm.CollectCardsFrom(new List<ICard>())).IgnoreArguments().Return(new List<ICard>());
            var concreteGameManager = new GameManager();
            _mockGameManager.Stub(mgm => mgm.PlaceYourBets(_sut.RegisteredPlayers))
                .Do(GetConcretePlaceYourBetsDelegateFrom(concreteGameManager));
            _mockGameManager.Stub(mgm => mgm.DealInitialCards(_mockGroupOfCards, _sut.RegisteredPlayers, _sut.CurrentCards))
                .Do(GetConcreteDealInitialCardsDelegateFrom(concreteGameManager));

            _sut.PlaySingleGame();

            Assert.IsNotNull(_sut.VisibleCard);
        }

        [TestMethod]
        public void When_Single_Game_Is_Played_Without_Dealer_Blackjack_Should_Call_Game_Steps_In_Proper_Order()
        {
            var strictMockGameManager = MockRepository.GenerateStrictMock<IGameManager>();
            var sut = new Dealer(_mockGroupOfCards, strictMockGameManager, _dealerStrategy);
            sut.SetTableSettingsWith(_tableSettings);
            sut.CurrentCards.Add(_nullCard);
            sut.CurrentCards.Add(_nullCard);
            sut.Register(GetMockBlackjackPlayerWithMockHand());

            strictMockGameManager.Expect(mgm => mgm.PlaceYourBets(sut.RegisteredPlayers));
            strictMockGameManager.Expect(mgm => mgm.DealInitialCards(null, null, null)).IgnoreArguments();
            strictMockGameManager.Expect(mgm => mgm.PlayersPlay(null, null, null)).IgnoreArguments();
            strictMockGameManager.Expect(mgm => mgm.DealerPlays(null, null, null)).IgnoreArguments();
            strictMockGameManager.Expect(mgm => mgm.DeterminePlayerHandOutcomes(null, null)).IgnoreArguments();
            strictMockGameManager.Expect(mgm => mgm.PayoutOrCollectBetsFrom(null)).IgnoreArguments().Return(0);
            strictMockGameManager.Expect(mgm => mgm.SaveCurrentHandResultsOf(null)).IgnoreArguments();
            strictMockGameManager.Expect(mgm => mgm.CollectCardsFrom((List<IPlayer>) null)).IgnoreArguments().Return(new List<ICard>());
            strictMockGameManager.Expect(mgm => mgm.CollectCardsFrom((List<ICard>) null)).IgnoreArguments()
                .Return(new List<ICard>());
            strictMockGameManager.Expect(mgm => mgm.ClearHandsFrom(null)).IgnoreArguments();
            strictMockGameManager.Expect(mgm => mgm.DetermineToLeaveGameOrStay(null)).IgnoreArguments();

            strictMockGameManager.Replay();

            sut.PlaySingleGame();
            strictMockGameManager.VerifyAllExpectations();
        }

        [TestMethod]
        public void When_Single_Game_Is_Played_With_Dealer_Blackjack_Should_Skip_Player_Plays_And_Dealer_Plays_Game_Steps()
        {
            var strictMockGameManager = MockRepository.GenerateStrictMock<IGameManager>();
            var sut = new Dealer(_mockGroupOfCards, strictMockGameManager, _dealerStrategy);
            sut.SetTableSettingsWith(_tableSettings);
            var blackjackCardValueAssigner = new BlackjackCardValueAssigner();
            sut.CurrentCards.Add(new Card(CardType.Ace, CardSuit.Clubs, blackjackCardValueAssigner));
            sut.CurrentCards.Add(new Card(CardType.Ten, CardSuit.Clubs, blackjackCardValueAssigner));
            sut.Register(GetMockBlackjackPlayerWithMockHand());

            strictMockGameManager.Expect(mgm => mgm.PlaceYourBets(sut.RegisteredPlayers));
            strictMockGameManager.Expect(mgm => mgm.DealInitialCards(null, null, null)).IgnoreArguments();
            strictMockGameManager.Expect(mgm => mgm.DeterminePlayerHandOutcomes(null, null)).IgnoreArguments();
            strictMockGameManager.Expect(mgm => mgm.PayoutOrCollectBetsFrom(null)).IgnoreArguments().Return(0);
            strictMockGameManager.Expect(mgm => mgm.SaveCurrentHandResultsOf(null)).IgnoreArguments();
            strictMockGameManager.Expect(mgm => mgm.CollectCardsFrom((List<IPlayer>)null)).IgnoreArguments().Return(new List<ICard>());
            strictMockGameManager.Expect(mgm => mgm.CollectCardsFrom((List<ICard>)null)).IgnoreArguments()
                .Return(new List<ICard>());
            strictMockGameManager.Expect(mgm => mgm.ClearHandsFrom(null)).IgnoreArguments();
            strictMockGameManager.Expect(mgm => mgm.DetermineToLeaveGameOrStay(null)).IgnoreArguments();

            strictMockGameManager.Replay();

            sut.PlaySingleGame();
            strictMockGameManager.VerifyAllExpectations();
        }

        [TestMethod]
        public void When_Single_Game_Is_Played_Should_Add_Player_Hand_Cards_And_Dealer_Cards_Back_To_Deck()
        {
            var mockGameManager = MockRepository.GenerateMock<IGameManager>();
            var sut = new Dealer(_mockGroupOfCards, mockGameManager, _dealerStrategy);
            sut.SetTableSettingsWith(_tableSettings);
            var dealerCards = new List<ICard> {_nullCard, _nullCard};
            sut.CurrentCards.AddRange(dealerCards);
            sut.Register(GetMockBlackjackPlayerWithMockHand());
            
            List<ICard> cardsToReturn = new List<ICard> { new NullCard() };
            mockGameManager.Stub(mgm => mgm.CollectCardsFrom(sut.RegisteredPlayers)).Return(cardsToReturn);
            mockGameManager.Stub(mgm => mgm.CollectCardsFrom(sut.CurrentCards)).Return(dealerCards);

            sut.PlaySingleGame();
            _mockGroupOfCards.AssertWasCalled(mgoc => mgoc.Cards.AddRange(cardsToReturn));
            _mockGroupOfCards.AssertWasCalled(mgoc => mgoc.Cards.AddRange(dealerCards));
        }

        private Action<IGroupOfCards, List<IPlayer>, List<ICard>> GetConcreteDealInitialCardsDelegateFrom(GameManager concreteGameManager)
        {
            return concreteGameManager.DealInitialCards;
        }

        private Action<List<IPlayer>> GetConcretePlaceYourBetsDelegateFrom(GameManager concreteGameManager)
        {
            return concreteGameManager.PlaceYourBets;
        }

        private IPlayer GetMockBlackjackPlayerWithMockHand()
        {
            var hands = new List<IPlayerHand>();
            var blackjackPlayer = MockRepository.GenerateMock<IPlayer>();
            blackjackPlayer.Stub(bp => bp.CurrentHands).Return(hands);

            return blackjackPlayer;
        }
    }
}
