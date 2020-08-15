using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static com.Rummy.GameVariable.GameVariables;

namespace com.Rummy.Ui
{
    public class RoomJoinUiController : MonoBehaviour
    {
        [Header("Room Type Selection Screen")]
        [SerializeField] private GameObject roomTypeSelectionScreen;
        [SerializeField] private Button randomRoomButton;
        [SerializeField] private Button customRoomButton;
        [SerializeField] private Button roomTypeSelectionScreenCloseButton;
        [Header("Room Size Selection Screen")]
        [SerializeField] private GameObject roomSizeSelectionScreen;
        [SerializeField] private Button roomSizeSelectionScreenCloseButton;
        [Header("Room Join Waiting Screen")]
        [SerializeField] private GameObject roomJoinWaitingScreen;
        [SerializeField] private Button roomJoinWaitingScreenCloseButton;

        private RoomType userSelectedRoom;

        private void Start()
        {
            randomRoomButton.onClick.AddListener(OnClickRandomRoomButton);
            customRoomButton.onClick.AddListener(OnClickCustomRoomButton);
            roomTypeSelectionScreenCloseButton.onClick.AddListener(OnClickroomTypeSelectionScreenCloseButton);
        }

        private void OnClickRandomRoomButton()
        {
            userSelectedRoom = RoomType.RandomRoom;
        }

        private void OnClickCustomRoomButton()
        {
            userSelectedRoom = RoomType.CustomRoom;
        }

        private void OnClickroomTypeSelectionScreenCloseButton()
        {
            gameObject.SetActive(false);
            roomTypeSelectionScreen.SetActive(false);
        }

        internal void EnableRoomTypeSelectionPanel()
        {
            gameObject.SetActive(true);
            roomTypeSelectionScreen.SetActive(true);
        }
    }
}
