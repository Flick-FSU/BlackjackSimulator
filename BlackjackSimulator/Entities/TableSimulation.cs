using System;
using System.Collections.Generic;
using System.Linq;
using BlackjackSimulator.Interfaces;
using BlackjackSimulator.Models;

namespace BlackjackSimulator.Entities
{
    public class TableSimulation : ITableSimulation
    {
        public IDealer Dealer { get; }
        public List<IPlayer> SeatedPlayers { get; }
        public int NumberOfPlayers => SeatedPlayers.Count;

        private readonly TableSettings _tableSettings;

        public TableSimulation(IDealer dealer, TableSettings tableSettings)
        {
            Dealer = dealer;
            Dealer.SetTableSettingsWith(tableSettings);
            SeatedPlayers = new List<IPlayer>();
            _tableSettings = tableSettings;
        }

        public void Seat(IPlayer player)
        {
            if (SeatedPlayers.Count >= _tableSettings.MaximumPlayerCount)
                throw new OverflowException("Table already at maximum number of players");

            player.JoinTableWith(Dealer);
            SeatedPlayers.Add(player);
        }

        public List<IPlayer> RunSimulationUntilAllPlayersUnregister()
        {
            if (!SeatedPlayers.Any())
                throw new InvalidOperationException("Cannot run simulation without seated players");

            while (Dealer.RegisteredPlayers.Any())
                Dealer.PlaySingleGame();

            return SeatedPlayers;
        }
    }
}