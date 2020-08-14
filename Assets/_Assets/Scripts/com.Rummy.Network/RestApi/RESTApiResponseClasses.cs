using System;
using System.Collections.Generic;

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

    public class RoomCreateResponse : ResponseData
    {
        public int room_id;
        public int time_remaining;
        public string room_code;
    }

    [Serializable]
    public class UserUpdateProfile : ResponseData
    {
        public int user_id;
        public int mob_no;
        public string email;
        public string user_name;
        public string first_name;
        public string last_name;
        public bool is_mob_verified;
        public bool is_email_verified;
        public int coins;
        public int balance;
    }

    [Serializable]
    public class UserGetProfile : ResponseData
    {
        public long user_id;
        public long mob_no;
        public string email;
        public string user_name;
        public string first_name;
        public string last_name;
        public bool is_mob_verified;
        public bool is_email_verified;
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
}