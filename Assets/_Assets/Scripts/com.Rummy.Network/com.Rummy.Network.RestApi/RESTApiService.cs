using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using System;
using Newtonsoft.Json;

namespace com.Rummy.Network
{
    //TODO Response validation.
    public enum BadRequestType
    {
        None = 0,
        LoggedInOtherDevice = 10,
        ServerDown = 11,
        MethodNotFound = 201,
        SomethingWentWrong = 12,
        NoInternet = 13,
        ParameterMandatory = 207,
        InvalidAuthToken = 204,
        ParamShouldNotBeEmpty = 208,
        InvalidBool = 209,
        InvaildInputInteger = 210,
        InvalidInputString = 211,
        InsufficientResouce = 230,
    };

    public class RESTApiService
    {
        // Generic method to send your request to server through POST method.
        internal static IEnumerator UnityWebRequestInPostMethod<T>(string baseUrl, Dictionary<string, string> payLoadKeyValuePairs, Action<string, string> errorResponse = null, Action<T> successResponse = null,bool shouldPrintResponseString = true)
        {
            UnityWebRequest unityWebRequest;
            var bodyJsonString               = SerializeDictionary(payLoadKeyValuePairs);
            unityWebRequest                  = UnityWebRequest.Post(baseUrl, bodyJsonString);
            unityWebRequest.method           = UnityWebRequest.kHttpVerbPOST;
            unityWebRequest.downloadHandler  = new DownloadHandlerBuffer();
            unityWebRequest.uploadHandler    = new UploadHandlerRaw(Encoding.UTF8.GetBytes(bodyJsonString));
            unityWebRequest.SetRequestHeader("Content-Type", "application/json");

            yield return unityWebRequest.SendWebRequest();

            if (unityWebRequest.isNetworkError)
            {
                errorResponse.Invoke(baseUrl, unityWebRequest.error);
            }
            else
            {
                if(shouldPrintResponseString)
                {
                    Debug.Log($"<Color=blue>Base Url : {baseUrl} \n Response : {unityWebRequest.downloadHandler.text}</Color>");
                }

                T response = Deserialize<T>(unityWebRequest.downloadHandler.text);
                successResponse.Invoke(response);
            }
        }

        // Generic method to send your request to server through GET method.
        internal static IEnumerator UnityWebRequestInGetMethod<T>(string baseUrl, Dictionary<string, string> payLoadKeyValuePairs, Action<string, string> errorResponse = null, Action<T> successResponse = null, bool shouldPrintResponseString = true)
        {
            StringBuilder url = new StringBuilder();
            url.Append(baseUrl).Append("?");

            foreach (var val in payLoadKeyValuePairs)
            {
                url.Append(val.Key).Append("=").Append(val.Value).Append("&");
            }
            url.Remove(url.Length - 1, 1);

            UnityWebRequest unityWebRequest = UnityWebRequest.Get(url.ToString());

            yield return unityWebRequest.SendWebRequest();

            if (unityWebRequest.isNetworkError)
            {
                errorResponse?.Invoke(baseUrl, unityWebRequest.error);
            }
            else
            {
                if (shouldPrintResponseString)
                {
                    Debug.Log($"<Color=blue>Base Url : {baseUrl} \n Response : {unityWebRequest.downloadHandler.text}</Color>");
                }

                T response = Deserialize<T>(unityWebRequest.downloadHandler.text);
                successResponse?.Invoke(response);
            }
        }

        private static string SerializeObject(object o)
        {
            return JsonUtility.ToJson(o);
        }

        private static string SerializeDictionary(Dictionary<string, string> payLoadKeyValuePairs)
        {
            return JsonConvert.SerializeObject(payLoadKeyValuePairs);
        }

        private static T Deserialize<T>(string msg)
        {
            return JsonUtility.FromJson<T>(msg);
        }
    }
}