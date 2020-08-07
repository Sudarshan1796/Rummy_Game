using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.Rummy.Constants
{
    public static class GameConstants
    {
        #region SocketConstants
        public const string SOCKET_HOST_ADDRESS  = "54.90.44.5";
        public const string SOCKET_PORT_NUMBER   = "3000";
        public const string SOCKET_URL_PREFIX    = "http://";
        public const string SOCKET_URL_SUFFIX    = "/ socket.io /";
        public const string SOCKET_URL_SEPARATOR = ":";
        #endregion

        public const string USER_ID = "userId";
        public const string ACCESS_TOKEN = "accessToken";
    }
}
