using System;
using System.Collections.Generic;
using UnityEngine;
using com.Rummy.Constants;

namespace com.Rummy.Network
{
    public class RESTApiConnectionManager : MonoBehaviour
    {
        private static RESTApiConnectionManager instance;
        internal static RESTApiConnectionManager GetInstance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<RESTApiConnectionManager>();
                }
                return instance;
            }
        }

        private Dictionary<string, string> payloadKeyValuePairs = new Dictionary<string, string>();

        //internal void Example<T>(string val, Action<string, string> errorResponse = null, Action<T> successResponse = null)
        //{
        //    payloadKeyValuePairs.Clear();
        //    payloadKeyValuePairs.Add(GameConstants.ACCESS_TOKEN, "example");
        //    payloadKeyValuePairs.Add(GameConstants.USER_ID, "example");
        //    StartCoroutine(RESTApiService.UnityWebRequestInGetMethod(GameConstants.GetRestApiUrl(GameConstants.RESTApiType.login),
        //                                                             payloadKeyValuePairs, errorResponse, successResponse, true));
        //}
    }
}
