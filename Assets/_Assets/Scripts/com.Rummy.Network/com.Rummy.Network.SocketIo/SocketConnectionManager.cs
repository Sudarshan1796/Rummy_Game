using UnityEngine;
using System;
using BestHTTP.SocketIO;
using com.Rummy.Constants;

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

        #region UnityCallbacks

            void Start()
            {
                ConnectToSocket();
            }

            void OnDestroy()
            {
                socketManager.Close();
            }

        #endregion

        internal void ConnectToSocket()
        {
            SocketOptions options = new SocketOptions();
            options.AutoConnect = false;

            socketManager = new SocketManager(new Uri(GetSocketUrl()), options);
            socketManager.Socket.On(SocketIOEventTypes.Connect, OnConnect);
            socketManager.Socket.On(SocketIOEventTypes.Disconnect, OndisConnect);
            socketManager.Socket.On(SocketIOEventTypes.Error, OnError);
            //socketManager.Socket.On("OnResponse", OnResponse);
            socketManager.Socket.On("userRoomJoin", OnResponse);
            socketManager.Open();
        }

        private string GetSocketUrl()
        {
            return GameConstants.SOCKET_URL_PREFIX + GameConstants.SOCKET_HOST_ADDRESS + GameConstants.SOCKET_URL_SEPARATOR + GameConstants.SOCKET_PORT_NUMBER + GameConstants.SOCKET_URL_SUFFIX;
        }

        private void OnConnect(Socket socket, Packet packet, params object[] args)
        {
            Debug.Log("<color=green>Socket Connected Successfully!</color>");
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

        private void SendSocketRequest(GameConstants.SocketRequestType requestType, Request requestObject)
        {
            socketManager.Socket.Emit(requestType.ToString(), Serialize(requestObject));

            string Serialize(object o)
            {
                return JsonUtility.ToJson(o);
            }
        }

        private void OnResponse(Socket socket, Packet packet, params object[] args)
        {
            Debug.Log(args[0]);
        }
    }
}
