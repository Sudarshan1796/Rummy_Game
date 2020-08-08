using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.Rummy.Gameplay
{
    public class GamePlayManager : MonoBehaviour
    {
        private static GamePlayManager instance;

        public static GamePlayManager GetInstance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<GamePlayManager>();
                }
                return instance;
            }
        }
    }
}