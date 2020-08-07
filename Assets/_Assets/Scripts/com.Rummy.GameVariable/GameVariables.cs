using com.Rummy.Constants;

namespace com.Rummy.GameVariable
{
    public static class GameVariables
    {
        public enum SocketRequestType : short
        {
            roomJoin,
            cardDraw,
            cardDiscard,
            show,
            playerLeft,
            drop
        }

        public enum SocketResponseType : short
        {
            onRoomJoin,
            userRoomJoin,
            gameStart,
            cardDrawRes,
            cardDiscardRes,
            playerLeftRes,
            roundComplete
        }

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
                return GameConstants.RESTAPI_URL_PREFIX + GameConstants.RESTAPI_HOST_ADDRESS + GameConstants.RESTAPI_URL_SEPARATOR + GameConstants.RESTAPI_PORT_NUMBER;
            }
        }

        internal static string GetSocketUrl()
        {
            return GameConstants.SOCKET_URL_PREFIX + GameConstants.SOCKET_HOST_ADDRESS + GameConstants.SOCKET_URL_SEPARATOR + GameConstants.SOCKET_PORT_NUMBER + GameConstants.SOCKET_URL_SUFFIX;
        }
    }
}
