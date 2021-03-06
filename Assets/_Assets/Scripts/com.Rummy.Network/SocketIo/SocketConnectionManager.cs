﻿using UnityEngine;
using System;
using BestHTTP.SocketIO;
using com.Rummy.GameVariable;
using System.Collections.Generic;
using com.Rummy.Constants;
using com.Rummy.Gameplay;

namespace com.Rummy.Network
{
    public class SocketConnectionManager : MonoBehaviour
    {
        private static SocketConnectionManager instance;
        internal static SocketConnectionManager GetInstance
        {
            get
            {
                if(instance == null)
                {
                    instance = FindObjectOfType<SocketConnectionManager>();
                }
                return instance;
            }
        }

        private SocketManager socketManager;
        private readonly object lockResponseInputQ = new object();
        private readonly LinkedList<SocketResponse> responseInputQ = new LinkedList<SocketResponse>();
        private Action onSocketConnect;

        internal event Action<SocketResponse> SocketResponse;

        #region  Properties

        public bool IsConnected
        {
            get
            {
                return socketManager.Socket.IsOpen;
            }
        }

        #endregion

        #region UnityCallbacks

        private void Update()
        {
            lock (lockResponseInputQ)
            {
                while (responseInputQ.Count > 0)
                {
                    if (responseInputQ.First.Value != null)
                    {
                        SocketResponse?.Invoke(responseInputQ.First.Value);
                        responseInputQ.RemoveFirst();
                    }
                }
            }
        }

        void OnDestroy()
        {
            socketManager?.Close();
        }

        #endregion

        internal void ConnectToSocket(Action onConnect)
        {
            SocketOptions options = new SocketOptions
            {
                AdditionalQueryParams = new PlatformSupport.Collections.ObjectModel.ObservableDictionary<string, string>
                {
                    { GameConstants.USER_ID, GameVariables.userId },
                    { GameConstants.ACCESS_TOKEN, GameVariables.AccessToken }
                }
            };

            onSocketConnect = onConnect;
            socketManager = new SocketManager(new Uri(GameVariables.GetSocketUrl()), options);
            socketManager.Socket.On(SocketIOEventTypes.Connect, OnConnect);
            socketManager.Socket.On(SocketIOEventTypes.Disconnect, OndisConnect);
            socketManager.Socket.On(SocketIOEventTypes.Error, OnError);

            socketManager.Socket.On(GameVariables.SocketResponseType.onRoomJoin.ToString(), (Socket socket, Packet packet, object[] args) => 
            { QueueResponse(Deserialize<OnRoomJoinResponse>(GameVariables.SocketResponseType.onRoomJoin, args[0] as string)); });

            socketManager.Socket.On(GameVariables.SocketResponseType.userRoomJoin.ToString(), (Socket socket, Packet packet, object[] args) =>
            { QueueResponse(Deserialize<UserRoomJoinResponse>(GameVariables.SocketResponseType.userRoomJoin, args[0] as string)); });

            socketManager.Socket.On(GameVariables.SocketResponseType.gameStart.ToString(), (Socket socket, Packet packet, object[] args) => 
            { QueueResponse(Deserialize<GameStartResponse>(GameVariables.SocketResponseType.gameStart, args[0] as string)); });

            socketManager.Socket.On(GameVariables.SocketResponseType.cardDrawRes.ToString(), (Socket socket, Packet packet, object[] args) => 
            { QueueResponse(Deserialize<CardDrawRes>(GameVariables.SocketResponseType.cardDrawRes, args[0] as string)); });
            
            socketManager.Socket.On(GameVariables.SocketResponseType.cardDiscardRes.ToString(), (Socket socket, Packet packet, object[] args) => 
            { QueueResponse(Deserialize<CardDiscardResResponse>(GameVariables.SocketResponseType.cardDiscardRes, args[0] as string)); });
            
            socketManager.Socket.On(GameVariables.SocketResponseType.playerLeftRes.ToString(), (Socket socket, Packet packet, object[] args) =>
            { QueueResponse(Deserialize<PlayerLeftResResponse>(GameVariables.SocketResponseType.playerLeftRes, args[0] as string)); });
            
            socketManager.Socket.On(GameVariables.SocketResponseType.roundComplete.ToString(), (Socket socket, Packet packet, object[] args) => 
            { QueueResponse(Deserialize<RoundCompleteResponse>(GameVariables.SocketResponseType.roundComplete, args[0] as string)); });

            socketManager.Socket.On(GameVariables.SocketResponseType.declareRes.ToString(), (Socket socket, Packet packet, object[] args) =>
            { QueueResponse(Deserialize<DeclarResponse>(GameVariables.SocketResponseType.declareRes, args[0] as string)); });

            socketManager.Socket.On(GameVariables.SocketResponseType.dropRes.ToString(), (Socket socket, Packet packet, object[] args) =>
            { QueueResponse(Deserialize<DropResponse>(GameVariables.SocketResponseType.dropRes, args[0] as string)); });

            socketManager.Socket.On(GameVariables.SocketResponseType.roomStateRes.ToString(), (Socket socket, Packet packet, object[] args) =>
            { QueueResponse(Deserialize<RoomStatusResponse>(GameVariables.SocketResponseType.roomStateRes, args[0] as string)); });

            socketManager.Socket.On(GameVariables.SocketResponseType.handValidateRes.ToString(), (Socket socket, Packet packet, object[] args) =>
            { QueueResponse(Deserialize<GroupValidationResponse>(GameVariables.SocketResponseType.handValidateRes, args[0] as string)); });

            socketManager.Socket.On(GameVariables.SocketResponseType.roomClose.ToString(), (Socket socket, Packet packet, object[] args) =>
            { QueueResponse(Deserialize<RoomCloseResponse>(GameVariables.SocketResponseType.roomClose, args[0] as string)); });

            socketManager.Socket.On(GameVariables.SocketResponseType.changeTurn.ToString(), (Socket socket, Packet packet, object[] args) =>
            { QueueResponse(Deserialize<PlayerTurnResponse>(GameVariables.SocketResponseType.changeTurn, args[0] as string)); });

            socketManager.Socket.On(GameVariables.SocketResponseType.matchComplete.ToString(), (Socket socket, Packet packet, object[] args) =>
            { QueueResponse(Deserialize<MatchCloseResponse>(GameVariables.SocketResponseType.matchComplete, args[0] as string)); });

            socketManager.Open();
        }

        private void OnConnect(Socket socket, Packet packet, params object[] args)
        {
            Debug.Log("<color=green>Socket Connected Successfully!</color>");
            onSocketConnect?.Invoke();
            //if (onSocketConnect == null && GamePlayManager.GetInstance.IsPlayinGame)
            //{
            //   // GamePlayManager.GetInstance.JoinDisconnectRoom();
            //}
            //onSocketConnect = null;
        }

        private void OndisConnect(Socket socket, Packet packet, params object[] args)
        {
            Debug.Log("<color=red>Socket Disconnected!</color>");
        }

        private void OnError(Socket socket, Packet packet, params object[] args)
        {
            Error error = args[0] as Error;
            switch (error.Code)
            {
                case SocketIOErrors.BadHandshakeMethod:
                    Debug.Log("<color=red>Bad handshake method!</color>");
                    break;
                case SocketIOErrors.BadRequest:
                    Debug.Log("<color=red>Bad request!</color>");
                    break;
                case SocketIOErrors.Custom:
                    Debug.Log("<color=red>A custom, server sent error, most probably from Socket.IO middleware!</color>");
                    break;
                case SocketIOErrors.Internal:
                    Debug.Log("<color=red>Internal error!</color>");
                    break;
                case SocketIOErrors.UnknownSid:
                    Debug.Log("<color=red>Session ID unknown!</color>");
                    break;
                case SocketIOErrors.UnknownTransport:
                    Debug.Log("<color=red>Transport unknown!</color>");
                    break;
                case SocketIOErrors.User:
                    Debug.Log("<color=red>Exception in an event handler!</color>");
                    break;
                default:
                    Debug.Log("<color=red>Server error!</color>");
                    break;
            }
            Debug.Log($"<color=red>Error Message : {error.Message}</color>");
        }

        T Deserialize<T>(GameVariables.SocketResponseType responseType, string msg) where T : SocketResponse
        {
            Debug.Log("******* Response From Socket *******");
            Debug.Log(responseType + "---" + msg);
            var obj = JsonUtility.FromJson<T>(msg);
            obj.socketResponseType = responseType;
            return obj;
        }

        void QueueResponse(SocketResponse response)
        {
            lock (lockResponseInputQ)
                responseInputQ.AddLast(response);
        }

        internal void SendSocketRequest(GameVariables.SocketRequestType requestType, SocketRequest requestObject)
        {
            Debug.Log("********** Sending Socket Request **********");
            Debug.Log(requestType + ":" + JsonUtility.ToJson(requestObject));
            socketManager.Socket.Emit(requestType.ToString(), JsonUtility.ToJson(requestObject));
        }

        internal void DisconnectSocket()
        {
            socketManager.Close();
        }
    }
}
