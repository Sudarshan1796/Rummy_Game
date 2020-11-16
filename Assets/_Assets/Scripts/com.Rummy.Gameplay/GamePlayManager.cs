using com.Rummy.GameVariable;
using com.Rummy.Network;
using com.Rummy.Ui;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.Rummy.Gameplay
{
    public class GamePlayManager : MonoBehaviour
    {
        #region Events
        internal event Action<List<PlayerCard>> OnGameStart;

        #endregion

        private static GamePlayManager instance;

        // This List hold the List of player in the current room
        internal List<Player> roomPlayers;
        internal List<PlayerCard> playerCards;

        internal Network.Card closedCard;
        internal Network.Card discardedCard;

        internal int playerTurn;
        internal int roomId;
        internal int remainingTime;
        internal int eventTime;
        internal bool isJoinedRoom;

        internal bool isCardDrawn = false;

        public static GamePlayManager GetInstance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<GamePlayManager>();
                }
                return instance;
            }
        }
        private void Awake()
        {
            roomPlayers = new List<Player>();
            playerCards = new List<PlayerCard>();
            //discardedPile = new DiscardPile();
        }

        private void OnEnable()
        {
            SocketConnectionManager.GetInstance.SocketResponse += OnSocketResponseReceived;
        }

        private void OnDisable()
        {
            if(SocketConnectionManager.GetInstance)
            SocketConnectionManager.GetInstance.SocketResponse -= OnSocketResponseReceived;
        }

        /// <summary>
        /// OnSocket response received 
        /// </summary>
        /// <param name="response"></param>
        private void OnSocketResponseReceived(SocketResponse response)
        {
            switch (response.socketResponseType)
            {
                case GameVariables.SocketResponseType.onRoomJoin: OnRoomJoin((OnRoomJoinResponse)response);
                    break;
                case GameVariables.SocketResponseType.userRoomJoin: UserRoomJoin((UserRoomJoinResponse)response);
                    break;
                case GameVariables.SocketResponseType.gameStart: GameStart((GameStartResponse)response);
                    break;
                case GameVariables.SocketResponseType.cardDrawRes: OncardDraw((CardDrawRes)response);
                    break;
                case GameVariables.SocketResponseType.cardDiscardRes: OnCardDiscard((CardDiscardResResponse)response);
                    break;
                case GameVariables.SocketResponseType.playerLeftRes: OnPlayerLeft((PlayerLeftResResponse)response);
                    break;
                case GameVariables.SocketResponseType.roundComplete: OnRoundComplete((RoundCompleteResponse)response);
                    break;
            }
        }
        #region Socket 

        #region Recieved Events
        /// <summary>
        /// when you join any room 
        /// </summary>
        private void OnRoomJoin(OnRoomJoinResponse response)
        {
            isJoinedRoom = true;
            roomPlayers.Clear();
            roomPlayers = response.players;
            roomId = response.roomId;
            Debug.Log(roomId + ":" + response.roomId);
            UiManager.GetInstance.SetRoomJoinDetails(response.players);
            UiManager.GetInstance.PrintRoomJoinedPlayersCount(response.players.Count);
        }

        /// <summary>
        /// when some one joins the room
        /// </summary>
        private void UserRoomJoin(UserRoomJoinResponse response)
        {
            Player _player = new Player
            {
                userId = response.userId,
                userName = response.userName,
                position = response.position
            };
            roomPlayers.Add(_player);
            UiManager.GetInstance.OnPlayerJoinRoom(_player);
            UiManager.GetInstance.PrintRoomJoinedPlayersCount(roomPlayers.Count);
            UiManager.GetInstance.PrintRoomJoinedPlayerRoom(response.userName);
        }

        private void GameStart(GameStartResponse response)
        {
            playerCards.Clear();
            playerCards = response.playerCards;
            playerTurn = response.playerTurn;
            remainingTime = response.remainingTime;
            eventTime = response.eventTime;
            UiManager.GetInstance.EnableGameplayScreen();
            OnGameStart?.Invoke(playerCards);
            discardedCard = response.discardPile;
            closedCard = response.closedDeck;
            UiManager.GetInstance.StartTimer(playerTurn,remainingTime,OnTimerComplete);
            isCardDrawn = false;
            UiManager.GetInstance.DisableRoomJoinWaitScreen();
        }
        
        private void OnTimerComplete()
        {

        }

        private void OncardDraw(CardDrawRes response)
        {
            //Move card to player position
            if (response.userId != int.Parse(GameVariables.userId))
            {
                UiManager.GetInstance.OtherplayerDrawCard();
            }
        }

        private void OnCardDiscard(CardDiscardResResponse response)
        {
            playerTurn = response.playerTurn;
            discardedCard = response.discardPile;
            closedCard = response.closedDeck;
            remainingTime = response.remainingTime;
            PlayerCard _playerCard = new PlayerCard
            {
                cardValue = discardedCard.cardValue,
                suitValue = discardedCard.suitValue
            };
            UiManager.GetInstance.MoveDiscardedCard(_playerCard, response.userId);
            UiManager.GetInstance.StartTimer(playerTurn, remainingTime, OnTimerComplete);
            isCardDrawn = false;
            //UiManager.GetInstance.StartTimer(playerTurn, remainingTime, OnTimerComplete);
        }

        private void OnPlayerLeft(PlayerLeftResResponse response)
        {
            // this is the temp code
            UiManager.GetInstance.EnableMainMenuUi();
            UiManager.GetInstance.DisableGamplayScreen();
            UiManager.GetInstance.LeaveSocketRoom();
        }

        private void OnRoundComplete(RoundCompleteResponse response)
        {
            //Show Results
        }
        #endregion

        #region SOCKET SEND

        internal void SocketRoomJoin(int roomId)
        {
            SocketConnectionManager.GetInstance.ConnectToSocket(() =>
            {
                RoomJoinRequest socketRequest = new RoomJoinRequest
                {
                    room_id = roomId,
                    user_id = int.Parse(GameVariables.userId),
                    access_token = GameVariables.AccessToken
                };
                SocketConnectionManager.GetInstance.SendSocketRequest(GameVariables.SocketRequestType.roomJoin, socketRequest);
            });
        }

        internal void DrawCard(bool isFromDiscardFile)
        {
            isCardDrawn = true;
            CardDrawRequest request = new CardDrawRequest
            {
                user_id = int.Parse(GameVariables.userId),
                room_id = roomId,
                is_from_discard_pile = isFromDiscardFile,
            };
            SocketConnectionManager.GetInstance.SendSocketRequest(GameVariables.SocketRequestType.cardDraw, request);
        }

        internal void DiscardCard(Network.Card drawCard)
        {
            CardDiscardRequest request = new CardDiscardRequest
            {
                user_id = int.Parse(GameVariables.userId),
                room_id = roomId,
                card = drawCard,
            };
            SocketConnectionManager.GetInstance.SendSocketRequest(GameVariables.SocketRequestType.cardDiscard, request);
        }

        internal void CardShow(List<Network.Card> cards)
        {
            ShowCardRequest request = new ShowCardRequest
            {
                user_id = int.Parse(GameVariables.userId),
                room_id = roomId,
                card_set = cards
            };
            SocketConnectionManager.GetInstance.SendSocketRequest(GameVariables.SocketRequestType.show, request);
        }

        internal void PlayerDrop()
        {
            DropRequest request = new DropRequest
            {
                user_id = int.Parse(GameVariables.userId),
            };
            SocketConnectionManager.GetInstance.SendSocketRequest(GameVariables.SocketRequestType.drop, request);
        }

        internal void PlayerLeft()
        {
            PlayerLeftRequest request = new PlayerLeftRequest
            {
                room_id = roomId,
                user_id = int.Parse(GameVariables.userId),
            };
            SocketConnectionManager.GetInstance.SendSocketRequest(GameVariables.SocketRequestType.show, request);
        }
        #endregion
        #endregion
    }
}