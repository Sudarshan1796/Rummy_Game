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
        public int responseCode;
        public T responseMsg;
        public string responseInfo;
    }
}