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
        internal DiscardPile discardedPile;

        internal int playerTurn;
        internal int roomId;
        internal int remainingTime;
        internal int eventTime;

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
        }

        private void OnEnable()
        {
            SocketConnectionManager.GetInstance.SocketResponse += OnSocketResponseReceived;
        }

        private void OnDisable()
        {
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
            roomPlayers.Clear();
            roomPlayers = response.players;
            roomId = response.roomId;
            Debug.Log(roomId + ":" + response.roomId);
            UiManager.GetInstance.SetRoomJoinDetails(response.players);
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
        }

        private void GameStart(GameStartResponse response)
        {
            playerCards.Clear();
            playerCards = response.playerCards;
            discardedPile = response.discardPile;
            playerTurn = response.playerTurn;
            remainingTime = response.remainingTime;
            eventTime = response.eventTime;
            OnGameStart?.Invoke(playerCards);
        }

        private void OncardDraw(CardDrawRes response)
        {

        }

        private void OnCardDiscard(CardDiscardResResponse response)
        {

        }

        private void OnPlayerLeft(PlayerLeftResResponse response)
        {

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