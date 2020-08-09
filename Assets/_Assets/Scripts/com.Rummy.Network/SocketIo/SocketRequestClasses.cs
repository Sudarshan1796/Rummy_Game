﻿using System;
using System.Collections.Generic;
using com.Rummy.GameVariable;

namespace com.Rummy.Network
{
    public class SocketRequest
    {

    }

    public class RoomJoinRequest: SocketRequest
    {
        public int user_id;
        public string access_token;
        public int room_id;
    }

    public class CardDrawRequest: SocketRequest
    {
        public int user_id;
        public int room_id;
        public bool is_from_discard_pile;
    }

    [Serializable]
    public class Card
    {
        public GameVariables.CardType cardValue;
        public GameVariables.SuitType suitValue;
    }

    public class CardDiscardRequest: SocketRequest
    {
        public int user_id;
        public int room_id;
        public Card card;
    }

    public class ShowCardRequest: SocketRequest
    {
        public int user_id;
        public int room_id;
        public List<Card> card_set;
    }

    public class PlayerLeftRequest: SocketRequest
    {
        public int room_id;
        public int user_id;
    }
}
