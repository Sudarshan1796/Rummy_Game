using System.Collections.Generic;
using com.Rummy.GameVariable;

namespace com.Rummy.Network
{
    public class SocketRequest
    {

    }

    public class RoomJoin: SocketRequest
    {
        public int user_id;
        public string access_token;
        public int room_id;
    }

    public class CardDraw: SocketRequest
    {
        public int user_id;
        public int room_id;
        public bool is_from_discard_pile;
    }

    public class Card
    {
        public GameVariables.CardType cardValue;
        public GameVariables.SuitType suitValue;
    }

    public class CardDiscard: SocketRequest
    {
        public int user_id;
        public int room_id;
        public Card card;
    }

    public class ShowCard: SocketRequest
    {
        public int user_id;
        public int room_id;
        public List<Card> card_set;
    }

    public class PlayerLeft: SocketRequest
    {
        public int room_id;
        public int user_id;
    }
}
