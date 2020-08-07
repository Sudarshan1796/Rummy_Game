namespace com.Rummy.Constants
{
    public static class GameConstants
    {
        #region SocketConstants

        internal const string SOCKET_URL_PREFIX    = "http://";
        internal const string SOCKET_HOST_ADDRESS  = "54.90.44.5";
        internal const string SOCKET_PORT_NUMBER   = "3000";
        internal const string SOCKET_URL_SUFFIX    = "/socket.io/";
        internal const string SOCKET_URL_SEPARATOR = ":";

        public enum SocketRequestType : short
        {
            roomJoin,
            cardDraw,
            cardDiscard,
            show,
            roomLeave,
            drop
        }

        #endregion

        #region RESTApiConstants

        internal const string RESTAPI_URL_PREFIX    = "http://";
        internal const string RESTAPI_HOST_ADDRESS  = "54.90.44.5";
        internal const string RESTAPI_PORT_NUMBER   = "3000";
        internal const string RESTAPI_URL_SEPARATOR = ":";

        public enum RESTApiType : short
        {
            login,
            getProfile,
            updateProfile,
            join,
            verify,
        }

        internal static string GetRestApiUrl(RESTApiType apiType)
        {
            switch (apiType)
            {
                case RESTApiType.login: return GetBaseUrl() + "/user/login";
                case RESTApiType.getProfile: return GetBaseUrl() + "/user/getProfile";
                case RESTApiType.updateProfile: return GetBaseUrl() + "/user/updateProfile";
                case RESTApiType.join: return GetBaseUrl() + "/room/join";
                case RESTApiType.verify: return GetBaseUrl() + "/user/verify";
                default: return null;
            };

            string GetBaseUrl()
            {
                return RESTAPI_URL_PREFIX + RESTAPI_HOST_ADDRESS + RESTAPI_URL_SEPARATOR + RESTAPI_PORT_NUMBER;
            }
        }

        internal const string USER_ID = "userId";
        internal const string ACCESS_TOKEN = "accessToken";

        #endregion
    }
}
