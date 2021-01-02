using com.Rummy.GameVariable;
using System;
using System.Collections.Generic;

namespace com.Rummy.Network
{
    public class SocketResponse
    {
        public GameVariables.SocketResponseType socketResponseType;
    }

    public class OnRoomJoinResponse: SocketResponse
    {
        public int roomId;
        public List<Player> players;
    }

    [Serializable]
    public class Player
    {
        public int userId;
        public string userName;
        public int position;
    }

    public class UserRoomJoinResponse: SocketResponse
    {
        public int userId;
        public string userName;
        public int position;
    }

    public class GameStartResponse: SocketResponse
    {
        public int roomId;
        public int playerTurn;
        public List<PlayerCard> playerCards;
        public Card discardPile;
        public Card closedDeck;
        public int eventTime;
        public int remainingTime;
        public Card wildCard;
    }

    [Serializable]
    public class DiscardPile
    {
        public GameVariables.CardType cardValue;
        public GameVariables.SuitType suitValue;
    }

    [Serializable]
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
        public Card discardPile;
        public Card closedDeck;
    }

    public class CardDiscardResResponse: SocketResponse
    {
        public int userId;
        public int position;
        public int roomId;
        public Card card;
        public int playerTurn;
        public int eventTime;
        public int remainingTime;
        public Card discardPile;
        public Card closedDeck;
    }

    public class PlayerLeftResResponse: SocketResponse
    {
        public int roomId;
        public int userId;
        public int position;
    }

   /* public class DeclareResponse: SocketResponse
    {
        public int roomId;
        public int userId;
        public int position;
        public string userName;
        public List<Card> cardSet;
        public int points;
    }*/
   [Serializable]
    public class RoundCompleteResponse: SocketResponse
    {
        public int roomId;
        public List<Result> result;
    }

    [Serializable]
    public class CardGroup
    {
        public int group_id;
        public List<Card> card_set;
    }

    public class DeclarResponse : SocketResponse
    {
        public int roomId;
        public int userId;
        public int position;
        public string userName;
        public int playerTurn;
        public int eventTime;
        public int remainingTime;
        public List<CardGroup> cardGroup;
        public int points;
        public int totalPoints;
        public int nextRoundStartTime;
        public Card showCard;
        public bool isWinner;
        public bool isEliminated;
        public List<GameResult> gameResult;

    }

    [Serializable]
    public class Result
    {
        public int userId;
        public int position;
        public string userName;
        public List<CardGroup> cardGroup;
        public int points;
        public int totalPoints;
        public bool isDropped;
        public bool isWinner;
        public bool isEliminated;
    }

    public class DropResponse : SocketResponse
    {
        public int userId;
        public int position;
        public int roomId;
        public int playerTurn;
        public int eventTime;
        public int remainingTime;
        public int nextRoundStartTime;
        public List<GameResult> gameResult;
    }

    [Serializable]
    public class GameResult
    {
        public int userId;
        public int position;
    }

    [Serializable]
    public class RoomInfo
    {
        public int userId;
        public int position;
        public string userName;
        public bool isDropped;
        public int status;
    }

    public class RoomStatusResponse : SocketResponse
    {
        public int roomId;
        public int nextEventTime;
        public int playerTurn;
        public int remainingTime;
        public List<Card> cardSet;
        public List<RoomInfo> roomInfo;
    }

    [Serializable]
    public class GroupValidation
    {
        public int groupId;
        public string handType;
    }

    public class GroupValidationResponse : SocketResponse
    {
        public List<GroupValidation> cardGroup;
    }

    public class PlayerTurnResponse : SocketResponse
    {
        public int userId;
        public int position;
        public int roomId;
        public int playerTurn;
        public int eventTime;
        public int remainingTime;
    }



}
