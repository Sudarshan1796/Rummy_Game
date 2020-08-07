using UnityEngine;
using System;
using BestHTTP.SocketIO;
using com.Rummy.Constants;

namespace com.Rummy.SocketIo
{
    public class SocketConnectionManager : MonoBehaviour
    {
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

        public void ConnectToSocket()
        {
            SocketOptions options = new SocketOptions
            {
                AutoConnect = false
            };

            socketManager = new SocketManager(new Uri(GetSocketUrl()), options);
            socketManager.Socket.On(SocketIOEventTypes.Connect, OnConnect);
            socketManager.Socket.On(SocketIOEventTypes.Disconnect, OndisConnect);
            socketManager.Socket.On(SocketIOEventTypes.Error, OnError);
            socketManager.Open();
        }

        private string GetSocketUrl()
        {
            string url = GameConstants.SOCKET_URL_PREFIX + GameConstants.SOCKET_HOST_ADDRESS + GameConstants.SOCKET_URL_SEPARATOR + GameConstants.SOCKET_PORT_NUMBER + GameConstants.SOCKET_URL_PREFIX;
            return url;
        }

        void OnConnect(Socket socket, Packet packet, params object[] args)
        {
            Debug.Log("Connected");
        }

        void OndisConnect(Socket socket, Packet packet, params object[] args)
        {
            Debug.Log("Disconnected");
        }

        void OnError(Socket socket, Packet packet, params object[] args)
        {
            Error error = args[0] as Error;

            switch (error.Code)
            {
                case SocketIOErrors.User:
                    Debug.LogWarning("Exception in an event handler!");
                    break;
                case SocketIOErrors.Internal:
                    Debug.LogWarning("Internal error!");
                    break;
                default:
                    Debug.LogWarning("server error!");
                    break;
            }
        }
    }
}
