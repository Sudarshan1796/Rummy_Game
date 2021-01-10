using com.Rummy.Network;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace com.Rummy.Gameplay
{
    public class PlayerUIController : MonoBehaviour
    {
        [SerializeField] private TMP_Text userName;
        [SerializeField] private Image userProfileImage;
        [SerializeField] private TimerController timerController;

        //for setting proper pos
        [SerializeField] public int uiIndexId;

        internal int userId;
        internal int position;

        private Color greenColor = new Color(0, 0.2352941f, 0.07843138f, 1.0f);

        internal void SetDetails(Player player)
        {
            userName.text = player.userName;
            userId = player.userId;
            position = player.position;
            userProfileImage.color = greenColor;
            gameObject.SetActive(true);
        }

        internal void DisableTimer()
        {
            gameObject.SetActive(false);
        }

        internal void SetTimer(int userId,float Timer,Action onComplete)
        {
            if (!gameObject.activeInHierarchy)
            {
                return;
            }
            if (userId == this.userId)
            {
                timerController.Activate(Timer, onComplete);
                if (userId == int.Parse(GameVariable.GameVariables.userId))
                {
#if UNITY_ANDROID && !UNITY_EDITOR
                    AndroidVibrationPlugin.Vibrate(255, 255);
#endif
                }
                ////gameObject.SetActive(true);
            }
            else
            {
                timerController.Deactiivate();
                //gameObject.SetActive(false);
            }
        }

        internal void GrayOutTimer()
        {
            userProfileImage.color = Color.grey;
        }

        internal void ResetTimerColor()
        {
            userProfileImage.color = greenColor;
        }

    }
}