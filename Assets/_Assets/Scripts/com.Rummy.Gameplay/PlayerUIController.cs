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

        internal int userId;
        internal int position;

        internal void SetDetails(Player player)
        {
            userName.text = player.userName;
            userId = player.userId;
            position = player.position;
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
                ////gameObject.SetActive(true);
            }
            else
            {
                timerController.Deactiivate();
                //gameObject.SetActive(false);
            }
        }
    }
}