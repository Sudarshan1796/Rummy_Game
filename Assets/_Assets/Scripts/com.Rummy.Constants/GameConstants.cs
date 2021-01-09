namespace com.Rummy.Constants
{
    public static class GameConstants
    {
        #region SocketConstants
        internal const string SOCKET_URL_PREFIX    = "http://";
        internal const string SOCKET_HOST_ADDRESS  = "34.207.57.181";
        internal const string SOCKET_PORT_NUMBER   = "3000";
        internal const string SOCKET_URL_SUFFIX    = "/socket.io/";
        internal const string SOCKET_URL_SEPARATOR = ":";
        internal const int ROOM_JOIN_CLOSE         = 0;
        internal const int ROOM_PLAYER_ACTIVE      = 1;
//exports.ROOM_PLAYER_DISCONNECT = 2;
//exports.ROOM_PLAYER_MATCH_COMPLETE = 3;
//exports.ROOM_PLAYER_FORCE_QUIT = 4;
//exports.ROOM_PLAYER_ELIMINATED = 5;
            #endregion

        #region RESTApiConstants

        internal const string RESTAPI_URL_PREFIX    = "http://";
        internal const string RESTAPI_HOST_ADDRESS  = "34.207.57.181";
        internal const string RESTAPI_PORT_NUMBER   = "3000";
        internal const string RESTAPI_URL_SEPARATOR = ":";

        internal const string USER_ID = "user_id";
        internal const string ACCESS_TOKEN = "access_token";
        internal const string MOB_NO = "mob_no";
        internal const string DEVICE_ID = "device_id";
        internal const string OTP = "otp";
        internal const string MAX_PLAYERS = "max_players";
        internal const string GAME_MODE = "game_mode";
        internal const string IS_PRACTICE = "is_practice";
        internal const string LAST_NAME = "last_name";
        internal const string FIRST_NAME = "first_name";
        internal const string USER_NAME = "user_name";
        internal const string EMAIL = "email";
        internal const string ROOM_CODE = "room_code";
        internal const string ENTRY_FEE = "entry_fee";
        internal const string Email = "email";

        #endregion
    }
}
