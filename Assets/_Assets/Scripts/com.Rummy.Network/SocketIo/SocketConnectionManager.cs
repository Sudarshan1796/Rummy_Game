using UnityEngine;
using System;
using BestHTTP.SocketIO;
using com.Rummy.GameVariable;
using System.Collections.Generic;

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
        private readonly List<SocketResponse> responseInputQ = new List<SocketResponse>();

        internal event Action<SocketResponse> SocketResponse;

        private Action onSocketConnect;
        #region UnityCallbacks

        void Start()
        {
            //ConnectToSocket();
        }

        private void Update()
        {
            lock (lockResponseInputQ)
            {
                while (responseInputQ.Count > 0)
                {
                    if (responseInputQ[0] != null)
                    {
                        OnSocketResponseReceived(responseInputQ[0]);
                        responseInputQ.RemoveAt(0);
                    }
                }
            }

            void OnSocketResponseReceived(SocketResponse response)
            {
                switch(response.socketResponseType)
                {
                    case GameVariables.SocketResponseType.onRoomJoin:
                        break;
                    case GameVariables.SocketResponseType.userRoomJoin:
                        break;
                    case GameVariables.SocketResponseType.gameStart:
                        break;
                    case GameVariables.SocketResponseType.cardDrawRes:
                        break;
                    case GameVariables.SocketResponseType.cardDiscardRes:
                        break;
                    case GameVariables.SocketResponseType.playerLeftRes:
                        break;
                    case GameVariables.SocketResponseType.roundComplete:
                        break;
                    default:
                        SocketResponse?.Invoke(response);
                        break;
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
                AutoConnect = false
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
            
            socketManager.Open();
        }

        private void OnConnect(Socket socket, Packet packet, params object[] args)
        {
            Debug.Log("<color=green>Socket Connected Successfully!</color>");
            onSocketConnect?.Invoke();
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
            var obj = JsonUtility.FromJson<T>(msg);
            obj.socketResponseType = responseType;
            return obj;
        }

        void QueueResponse(SocketResponse response)
        {
            lock (lockResponseInputQ)
                responseInputQ.Add(response);
        }

        internal void SendSocketRequest(GameVariables.SocketRequestType requestType, SocketRequest requestObject)
        {
            socketManager.Socket.Emit(requestType.ToString(), Serialize(requestObject));

            string Serialize(object o)
            {
                return JsonUtility.ToJson(o);
            }
        }
    }
}
