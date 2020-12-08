using System;
using System.Collections.Generic;
using com.Rummy.GameVariable;

namespace com.Rummy.Network
{
    [Serializable]
    public class UserLoginResponse : ResponseData
    {
        public int next_otp_time_limit;
    }

    [Serializable]
    public class UserVerifyResponse : ResponseData
    {
        public int user_id;
        public string access_token;
    }

    [Serializable]
    public class RoomJoinResponse : ResponseData
    {
        public int room_id;
        public int time_remaining;
    }

    [Serializable]
    public class RoomCreateResponse : ResponseData
    {
        public int room_id;
        public int time_remaining;
        public string room_code;
    }

    [Serializable]
    public class UserUpdateProfileResponse : ResponseData
    {
        public int user_id;
        public string email;
        public string temp_email;
        public string user_name;
        public string first_name;
        public string last_name;
    }

    [Serializable]
    public class UserGetProfileResponse : ResponseData
    {
        public long user_id;
        public long mob_no;
        public string email;
        public string user_name;
        public string temp_email;
        public int temp_mob_no;
        public string first_name;
        public string last_name;
        public int coins;
        public Balance balance;
        public List<AccountStatement> account_statement;
        public List<Withdraw> withdraw;
        public List<Td> tds;
        public KycDocuments kyc_documents;
    }

    [Serializable]
    public class KycDocuments
    {
        public string pan;
        public string bank;
    }

    [Serializable]
    public class Td
    {
        public int date;
        public string description;
        public int amount;
        public string status;
    }

    [Serializable]
    public class Withdraw
    {
        public string id;
        public string name;
        public string acc_number;
        public string ifsc;
        public int amount;
        public string status;
        public int date;
    }

    [Serializable]
    public class AccountStatement
    {
        public int date;
        public string description;
        public int amount;
        public Balance balance;
    }

    [Serializable]
    public class Balance
    {
        public int deposit;
        public int withdrawable;
    }

    [Serializable]
    public class RoomListResponse : ResponseData
    {
        public List<CashGameInfo> cashGameInfo;
        public List<PracticeGameInfo> practiceGameInfo;
    }

    [Serializable]
    public class PracticeGameInfo
    {
        public GameVariables.GameMode gameMode;
        public int minFee;
        public int maxFee;
        public int activePlayers;
        public List<RoomData> roomData;
    }

    [Serializable]
    public class CashGameInfo
    {
        public GameVariables.GameMode gameMode;
        public int minFee;
        public int maxFee;
        public int activePlayers;
        public List<RoomData> roomData;
    }

    [Serializable]
    public class RoomData
    {
        public int maxPlayers;
        public int entryFee;
        public int activePlayers;
        public int usersInTable;
    }
}