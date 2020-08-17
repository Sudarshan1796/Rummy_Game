using System;
using static com.Rummy.GameVariable.GameVariables;

namespace com.Rummy.Network
{
    [Serializable]
    public class ResponseData
    {
        public CodeType code;
    }

    [Serializable]
    public class RESTApiResponse<T> where T : ResponseData
    {
        public int responseCode;
        public T responseData;
        public string responseMessage;
    }
}