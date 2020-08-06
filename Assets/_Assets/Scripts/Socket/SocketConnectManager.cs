using UnityEngine;
using System;
using BestHTTP.SocketIO;

namespace com.Rummy.SocketIo
{
    public class SocketConnectManager : MonoBehaviour
    {
        private SocketManager manager;

        #region UnityCallbacks

            void Start()
            {
                ConnectToSocket();
            }

            void OnDestroy()
            {
                manager.Close();
            }

        #endregion

        public void ConnectToSocket()
        {
            SocketOptions options = new SocketOptions();
            options.AutoConnect = false;

            manager = new SocketManager(new Uri(GetSocketUrl()), options);
            manager.Socket.On(SocketIOEventTypes.Connect, OnConnect);
            manager.Socket.On(SocketIOEventTypes.Disconnect, OndisConnect);
            manager.Socket.On(SocketIOEventTypes.Error, OnError);
            manager.Open();
        }

        private string GetSocketUrl()
        {
            string url;
            url = SocketConstants.URL_PREFIX + SocketConstants.HOST_ADDRESS + SocketConstants.URL_SEPERATOR + SocketConstants.PORT_NUMBER + SocketConstants.URL_SUFFIX;
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
