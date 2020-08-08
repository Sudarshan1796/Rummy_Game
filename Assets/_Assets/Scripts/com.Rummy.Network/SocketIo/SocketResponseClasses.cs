using com.Rummy.GameVariable;
using System.Collections.Generic;

namespace com.Rummy.Network
{
    public class SocketResponse
    {
        public GameVariables.SocketResponseType socketResponseType;
    }

    public class OnRoomJoin: SocketResponse
    {
        public int roomId;
        public List<Player> players;
    }

    public class Player
    {
        public int userId;
        public string userName;
        public int position;
    }

    public class UserRoomJoin: SocketResponse
    {
        public int userId;
        public string userName;
        public int position;
    }

    public class GameStart: SocketResponse
    {
        public int roomId;
        public int playerTurn;
        public List<PlayerCard> playerCards;
        public DiscardPile discardPile;
        public int eventTime;
        public int remainingTime;
    }

    public class DiscardPile
    {
        public GameVariables.CardType cardValue;
        public GameVariables.SuitType suitValue;
    }

    public class PlayerCard
    {
        public GameVariables.CardType cardValue;
        public GameVariables.SuitType suitValue;
    }

    public class CardDrawRes: SocketResponse
    {
        public int userId;
        public int roomId;
        public Card card;
        public bool isFromDiscardPile;
        public DiscardPile discardPile;
    }

    public class CardDiscardRes: SocketResponse
    {
        public int userId;
        public int position;
        public int roomId;
        public Card card;
        public int playerTurn;
        public int eventTime;
        public int remainingTime;
        public DiscardPile discardPile;
    }

    public class PlayerLeftRes: SocketResponse
    {
        public int roomId;
        public int userId;
        public int position;
    }

    public class Declare: SocketResponse
    {
        public int roomId;
        public int userId;
        public int position;
        public string userName;
        public List<Card> cardSet;
        public int points;
    }

    public class RoundComplete: SocketResponse
    {
        public int roomId;
        public List<Result> result;
    }

    public class Result
    {
        public int userId;
        public int position;
        public string userName;
        public List<Card> cardSet;
        public int points;
    }
}
