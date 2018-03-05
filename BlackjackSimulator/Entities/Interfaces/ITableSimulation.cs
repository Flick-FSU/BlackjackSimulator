using System.Collections.Generic;

namespace BlackjackSimulator.Entities.Interfaces
{
    public interface ITableSimulation
    {
        IDealer Dealer { get; }
        void Seat(IPlayer player);
        List<IPlayer> RunSimulationUntilAllPlayersUnregister();
        int NumberOfPlayers { get; }
    }
}