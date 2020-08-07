using System;

namespace com.Rummy.Network
{
    [Serializable]
    public class ResponseMessage
    {
		
    }

    [Serializable]
    public class ResponseData<T> where T : ResponseMessage
    {
        public int responseType;
        public int responseCode;
        public T responseMsg;
        public string responseInfo;
    }

    [Serializable]
    public class MetaData
    {
        public string accessToken;
        public string serverTime;
    }
}