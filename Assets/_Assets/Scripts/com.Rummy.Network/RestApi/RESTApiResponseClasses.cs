using System;
using System.Collections.Generic;

namespace com.Rummy.Network
{
    public class UserLoginResponse : ResponseMessage
    {
        public int next_otp_time_limit;
    }

    public class UserVerifyResponse : ResponseMessage
    {
        public int user_id;
        public string access_token;
    }

    public class RoomJoinResponse : ResponseMessage
    {
        public int room_id;
        public int time_remaining;
    }

    public class UserUpdateProfile : ResponseMessage
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

    public class UserGetProfile : ResponseMessage
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