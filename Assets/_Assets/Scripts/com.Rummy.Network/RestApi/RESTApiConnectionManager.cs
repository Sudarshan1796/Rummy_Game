﻿using System;
using System.Collections.Generic;
using UnityEngine;
using com.Rummy.Constants;
using com.Rummy.GameVariable;

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

        //private void Start()
        //{
        //    UserLogin<UserLoginResponse>("8494844980", SystemInfo.deviceUniqueIdentifier);
        //}

        internal void UserLogin<T>(string mobNumber, string deviceId, Action<T> successResponse = null, Action<string, string> errorResponse = null) where T : ResponseData
        {
            payloadKeyValuePairs.Clear();
            payloadKeyValuePairs.Add(GameConstants.MOB_NO, mobNumber);
            payloadKeyValuePairs.Add(GameConstants.DEVICE_ID, deviceId);
            _ = StartCoroutine(RESTApiService.UnityWebRequestInPostMethod(GameVariables.GetRestApiUrl(GameVariables.RESTApiType.login),
                                                                          payloadKeyValuePairs, successResponse, errorResponse));
        }

        internal void UserVerify<T>(string mobNumber, string otp, Action<T> successResponse = null, Action<string, string> errorResponse = null) where T : ResponseData
        {
            payloadKeyValuePairs.Clear();
            payloadKeyValuePairs.Add(GameConstants.MOB_NO, mobNumber);
            payloadKeyValuePairs.Add(GameConstants.OTP, otp);
            _ = StartCoroutine(RESTApiService.UnityWebRequestInPostMethod(GameVariables.GetRestApiUrl(GameVariables.RESTApiType.verify),
                                                                          payloadKeyValuePairs, successResponse, errorResponse));
        }

        internal void UserUpdateProfile<T>(string mobNumber, string email, string userName, string firstName, string lastName, Action<T> successResponse = null, Action<string, string> errorResponse = null) where T : ResponseData
        {
            payloadKeyValuePairs.Clear();
            payloadKeyValuePairs.Add(GameConstants.USER_ID, GameVariables.userId);
            payloadKeyValuePairs.Add(GameConstants.ACCESS_TOKEN, GameVariables.AccessToken);
            payloadKeyValuePairs.Add(GameConstants.MOB_NO, mobNumber);
            payloadKeyValuePairs.Add(GameConstants.EMAIL, email);
            payloadKeyValuePairs.Add(GameConstants.USER_NAME, userName);
            payloadKeyValuePairs.Add(GameConstants.FIRST_NAME, firstName);
            payloadKeyValuePairs.Add(GameConstants.LAST_NAME, lastName);
            _ = StartCoroutine(RESTApiService.UnityWebRequestInPostMethod(GameVariables.GetRestApiUrl(GameVariables.RESTApiType.updateProfile),
                                                                          payloadKeyValuePairs, successResponse, errorResponse));
        }

        internal void UserGetProfile<T>(Action<T> successResponse = null, Action<string, string> errorResponse = null) where T : ResponseData
        {
            payloadKeyValuePairs.Clear();
            payloadKeyValuePairs.Add(GameConstants.USER_ID, GameVariables.userId);
            payloadKeyValuePairs.Add(GameConstants.ACCESS_TOKEN, GameVariables.AccessToken);
            _ = StartCoroutine(RESTApiService.UnityWebRequestInPostMethod(GameVariables.GetRestApiUrl(GameVariables.RESTApiType.getProfile),
                                                                          payloadKeyValuePairs, successResponse, errorResponse));
        }

        internal void RoomJoin<T>(bool isPractice, GameVariables.GameMode gameMode, int maxPlayers, Action<T> successResponse = null, Action<string, string> errorResponse = null) where T : ResponseData
        {
            payloadKeyValuePairs.Clear();
            payloadKeyValuePairs.Add(GameConstants.USER_ID, GameVariables.userId);
            payloadKeyValuePairs.Add(GameConstants.ACCESS_TOKEN, GameVariables.AccessToken);
            payloadKeyValuePairs.Add(GameConstants.IS_PRACTICE, isPractice?"1":"0");
            payloadKeyValuePairs.Add(GameConstants.GAME_MODE, gameMode.ToString());
            payloadKeyValuePairs.Add(GameConstants.MAX_PLAYERS, maxPlayers.ToString());
            _ = StartCoroutine(RESTApiService.UnityWebRequestInPostMethod(GameVariables.GetRestApiUrl(GameVariables.RESTApiType.join),
                                                                          payloadKeyValuePairs, successResponse, errorResponse));
        }
    }
}