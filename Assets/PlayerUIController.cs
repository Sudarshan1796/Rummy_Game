using com.Rummy.Network;
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
        }

        internal void SetTimer()
        {

        }
    }
}