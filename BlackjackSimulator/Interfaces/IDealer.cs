﻿using System.Collections.Generic;
using BlackjackSimulator.Models;
using GamblingLibrary.Interfaces;

namespace BlackjackSimulator.Interfaces
{
    public interface IDealer
    {
        List<ICard> CurrentCards { get; }
        List<IPlayer> RegisteredPlayers { get; }
        ICard VisibleCard { get; }
        TableSettings TableSettings { get; }

        void PlaySingleGame();
        void Register(IPlayer player);
        void UnregisterPlayer(IPlayer player);
        void SetTableSettingsWith(TableSettings tableSettings);
    }
}