using com.Rummy.GameVariable;
using com.Rummy.Network;
using com.Rummy.Ui;
using com.Rummy.UI;
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

        private bool _isPlayerDeclare;
        private bool _isPlayingGame;
        private bool _isOtherplayerDeclared;

        // This List hold the List of player in the current room
        internal List<Player> roomPlayers;
        internal List<PlayerCard> playerCards;

        internal Network.Card selectedCard;
        internal Network.Card closedCard;
        internal Network.Card discardedCard;
        internal Network.Card wildCard;

        internal int playerTurn;
        internal int roomId;
        internal int remainingTime;
        internal int eventTime;
        internal bool isJoinedRoom;
        internal bool isPlayerDropped;
        internal bool isCardDrawn = false;

        //This used to keep track of all group validation request
        private int groupValidationRequestCount = 0;

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

        public bool IsPlayerDeclare
        {
            get
            {
                return _isPlayerDeclare;
            }
            set
            {
                _isPlayerDeclare = value;
            }
        }

        public bool IsPlayinGame
        {
            get
            {
               return _isPlayingGame;
            }
            set
            {
                _isPlayingGame = value;
            }
        }

        public bool CanPlayerMakeCardMovement
        {
            get
            {
                return _isOtherplayerDeclared;
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
        /// Reset ALl gameplay data
        /// </summary>
        private void ResetGameplay()
        {
            _isPlayerDeclare = false;
        }

        /// <summary>
        /// For discconnect senario
        /// </summary>
        internal void JoinDisconnectRoom()
        {
            SocketRoomJoin(roomId);
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
                case GameVariables.SocketResponseType.declareRes:OnDeclarePlayer((DeclarResponse)response);
                    break;
                case GameVariables.SocketResponseType.dropRes: OnPlayerDrop((DropResponse)response);
                    break;
                case GameVariables.SocketResponseType.roomStateRes: OnRoomStatusResponse((RoomStatusResponse) response);
                    break;
                case GameVariables.SocketResponseType.handValidateRes:OnGroupValidationResponse((GroupValidationResponse)response);
                    break;
                case GameVariables.SocketResponseType.changeTurn:OnPlayerTurmChangeResponse((PlayerTurnResponse)response);
                    break;
                case GameVariables.SocketResponseType.roomClose: OnRoomClose((RoomCloseResponse)response);
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
            ResetGameplay();
            UiManager.GetInstance.DisablePlayerTimer();
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
            _isOtherplayerDeclared = false;
            groupValidationRequestCount = 0;
            _isPlayingGame = true;
            _isPlayerDeclare = false;
            playerCards.Clear();
            playerCards = response.playerCards;
            playerTurn = response.playerTurn;
            remainingTime = response.remainingTime;
            wildCard = response.wildCard;
            eventTime = response.eventTime;
            UiManager.GetInstance.EnableGameplayScreen();
            OnGameStart?.Invoke(playerCards);
            discardedCard = response.discardPile;
            closedCard = response.closedDeck;
            UiManager.GetInstance.StartTimer(playerTurn,remainingTime,OnTimerComplete);
            isCardDrawn = false;
            UiManager.GetInstance.DisableRoomJoinWaitScreen();
            CardGroupController.GetInstance.EnableDropButton();
            UiManager.GetInstance.DisableResultScreen();
            CardGroupController.GetInstance.EnableOpenPile();
        }

        private void OnTimerComplete()
        {

        }

        private void OncardDraw(CardDrawRes response)
        {
            //Move card to player position
            closedCard = response.closedDeck;
            if (response.discardPile.suitValue == GameVariables.SuitType.Joker && response.discardPile.cardValue == GameVariables.CardType.Joker)
            {
                discardedCard = null;
            }
            else
            {
                discardedCard = response.discardPile;

            }
            selectedCard = response.card;
            UiManager.GetInstance.OnCardDraw(response);
        }

        private void OnCardDiscard(CardDiscardResResponse response)
        {

            discardedCard = response.discardPile;
            playerTurn = response.playerTurn;
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
            CardGroupController.GetInstance.EnableDropButton(true);
            //UiManager.GetInstance.StartTimer(playerTurn, remainingTime, OnTimerComplete);
        }

        private void OnPlayerLeft(PlayerLeftResResponse response)
        {
            if (response.userId == int.Parse(GameVariables.userId))
            {
                UiManager.GetInstance.EnableMainMenuUi();
                UiManager.GetInstance.DisableGamplayScreen();
                UiManager.GetInstance.LeaveSocketRoom();
            }
            else
            {
                UiManager.GetInstance.OnPlayerLeft(response.userId);
            }
        }

        private void OnPlayerDrop(DropResponse response)
        {
            playerTurn      = response.playerTurn;
            remainingTime   = response.remainingTime;
            isPlayerDropped = (response.userId == int.Parse(GameVariables.userId));
            if (response.isLastRound)
            {
                UiManager.GetInstance.EnableResultScreen();
                ResultScreen.GetInstance?.UpdateNextMatchTimer(response.nextRoundStartTime);
                ResultScreen.GetInstance?.UpdatePlayerPosition(response.gameResult);
            }
            else
            {
                UiManager.GetInstance.OnPlayerLeft(response.userId);
            }
            //Todo: Make the Player just Spectacle
        }

        private void OnDeclarePlayer(DeclarResponse response)
        {
            if (_isPlayerDeclare)
            {
                if (int.Parse(GameVariables.userId) != response.userId)
                {
                    UiManager.GetInstance.SetDeclareScreeenData(response);
                }
                ResultScreen.GetInstance?.UpdateNextMatchTimer(response.nextRoundStartTime);
                ResultScreen.GetInstance?.UpdatePlayerPosition(response.gameResult);
                return;
            }
            if (int.Parse(GameVariables.userId) != response.userId)
            {
                _isOtherplayerDeclared = true;
                GameplayController.GetInstance.SetShowCard(response.showCard);
                UiManager.GetInstance.ConfirmationPoup("Are sure you want to Declare?", "Declare", Declare);
            }
            CardGroupController.GetInstance.EnableDropButton(true);
            CardGroupController.GetInstance.UpdateDeclareButtonState(true);
        }

        private void Declare()
        {
            CardGroupController.GetInstance.Declare();
        }

        private void OnRoundComplete(RoundCompleteResponse response)
        {
            //Show Results
            UiManager.GetInstance.EnableResultScreen();
            UiManager.GetInstance.SetResultScreeenData(response);
        }

        private void OnRoomStatusResponse(RoomStatusResponse response)
        {
            playerTurn = response.playerTurn;
            remainingTime = response.remainingTime;
            UiManager.GetInstance.StartTimer(playerTurn, remainingTime, OnTimerComplete);
            CardGroupController.GetInstance.EnableDropButton();
        }

        private void OnGroupValidationResponse(GroupValidationResponse response)
        {
            groupValidationRequestCount--;
            if (groupValidationRequestCount > 0)
            {
                return;
            }
            CardGroupController.GetInstance.ValidateGroupSequense(response);
        }

        private void OnPlayerTurmChangeResponse(PlayerTurnResponse response)
        {
            playerTurn = response.playerTurn;
            remainingTime = response.remainingTime;
            UiManager.GetInstance.StartTimer(playerTurn, remainingTime, OnTimerComplete);
        }

        private void OnRoomClose(RoomCloseResponse response)
        {
            _isPlayingGame = false;
            UiManager.GetInstance.EnableNoMatchFoundPopUp();
        }


        #endregion

        #region Client Event

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

        internal void PlayerDeclare(List<Network.CardGroup> groupset,Network.Card card)
        {
            ShowCardRequest request = new ShowCardRequest
            {
                user_id     = int.Parse(GameVariables.userId),
                room_id     = roomId,
                card_group  = groupset,
                show_card   = card,
            };
            SocketConnectionManager.GetInstance.SendSocketRequest(GameVariables.SocketRequestType.declare, request);
        }

        internal void PlayerDrop()
        {
            DropRequest request = new DropRequest
            {
                user_id = int.Parse(GameVariables.userId),
                room_id = roomId,
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
            SocketConnectionManager.GetInstance.SendSocketRequest(GameVariables.SocketRequestType.declare, request);
        }

        internal void RoomStatus()
        {
            RoomStatusRequest request = new RoomStatusRequest
            {
                user_id = int.Parse(GameVariables.userId),
                room_id = roomId,
            };
            SocketConnectionManager.GetInstance.SendSocketRequest(GameVariables.SocketRequestType.roomState, request);
        }

        internal void CardGroupValidation(List<Network.CardGroup> groupset)
        {
            CardGroupValidationRequest request = new CardGroupValidationRequest
            {
                room_id = roomId,
                card_group = groupset,
            };
            SocketConnectionManager.GetInstance.SendSocketRequest(GameVariables.SocketRequestType.handValidate, request);
            groupValidationRequestCount++;
        }
        #endregion
        #endregion

        #region  UNITY_CALLBACK

        private void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus && _isPlayingGame && SocketConnectionManager.GetInstance.IsConnected)
            {
#if !UNITY_EDITOR
                                RoomStatus();
#endif
            }
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (!pauseStatus && _isPlayingGame && SocketConnectionManager.GetInstance.IsConnected)
            {
                RoomStatus();
            }
        }

        #endregion
    }
}