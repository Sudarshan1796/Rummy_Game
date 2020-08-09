using System;

namespace com.Rummy.Network
{
    [Serializable]
    public class ResponseData
    {
		
    }

    [Serializable]
    public class RESTApiResponse<T> where T : ResponseData
    {
        public int responseCode;
        public T responseData;
        public string responseMessage;
    }
}