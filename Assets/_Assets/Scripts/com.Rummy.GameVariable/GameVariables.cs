using com.Rummy.Constants;
using com.Rummy.Network;

namespace com.Rummy.GameVariable
{
    public static class GameVariables
    {
        internal static string GetRestApiUrl(RESTApiType apiType)
        {
            switch (apiType)
            {
                case RESTApiType.login: return GetBaseUrl() + "/user/login";
                case RESTApiType.getProfile: return GetBaseUrl() + "/user/getProfile";
                case RESTApiType.updateProfile: return GetBaseUrl() + "/user/updateProfile";
                case RESTApiType.join: return GetBaseUrl() + "/room/join";
                case RESTApiType.create:return GetBaseUrl() + "/room/create";
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

        public static string userId;
        public static string AccessToken;
        public static UserGetProfile UserProfile;
        public static GameMode userSelectedGameMode;
        public static RoomType userSelectedRoomType;
        public static RoomSize userSelectedRoomSize;

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
            create,
        }

        public enum SuitType: short
        {
            Joker=0,
            Spades = 1,
            Hearts,
            Clubs,
            Diamonds
        }

        public enum CardType: short
        {
            Joker=0,
            Ace = 1,
            Two,
            Three,
            Four,
            Five,
            Six,
            Seven,
            Eight,
            Nine,
            Ten,
            Jack,
            Queen,
            King
        }

        public enum GameMode: short
        {
            Points = 1,
            Pool101,
            Deals,
            Pool201
        }

        public enum RoomType : short
        {
            RandomRoom,
            CustomRoom
        }

        public enum RoomSize : short
        {
            Players2 = 2,
            Players6 = 6
        }

        public enum SetType : short
        {
            pureSequence=1,
            ImpureSequence,
            Set,
            Invalid,
        }

        public enum CodeType : short
        {
            None = 0,
            InvalidEmail = 205,
            InvalidOtp = 206,
            RoomIsActive = 216,
            InvalidRoomId = 219,
        }
    }
}
