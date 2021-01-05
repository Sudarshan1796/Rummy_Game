using com.Rummy.Network;
using com.Rummy.Ui;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace com.Rummy.UI
{
    public class ResultScreen : MonoBehaviour
    {
        private static ResultScreen instance;

        public static ResultScreen GetInstance
        {
            get
            {
                if (instance==null)
                {
                    instance = FindObjectOfType<ResultScreen>();
                }

                return instance;
            }
        }
        [SerializeField] private List<PlayerResultPanel> resultPanels;

        [SerializeField] private TMP_Text remainingTimeText;
        [SerializeField] private Button homeButton;

        private int remaininMatchTime;
        private bool isTimerStarted;
        private bool _isEliminated;
        private void Awake()
        {
            homeButton.onClick.AddListener(OnHomeBtnClick);
        }

        private void OnDisable()
        {
            isTimerStarted = false;
        }

        /// <summary>
        /// To Enable and Disable the ResultScreen
        /// </summary>
        /// <param name="state"></param>
        internal void UpdateState(bool state)
        {
            gameObject.SetActive(state);
        }

        public void OnRoundComplete(RoundCompleteResponse response)
        {
            _isEliminated = false;
            homeButton.gameObject.SetActive(false);
            ResetPlayerPanel();
            for (int i = 0; i < response.result.Count; i++)
            {
                resultPanels[i].UpdateState(true);
                resultPanels[i].PlayerPanelReset();
                resultPanels[i].SetDetails(response.result[i]);
                if (response.result[i].isEliminated && response.result[i].userId == int.Parse(GameVariable.GameVariables.userId))
                {
                    _isEliminated = true;
                    //CancelInvoke(nameof(UpdateNextTimer));
                    remaininMatchTime = 0;
                    remainingTimeText.text = "";
                    LeanTween.delayedCall(3.0f, () =>
                    {
                        homeButton.gameObject.SetActive(true);
                    });
                }
            }
            for (int i = response.result.Count; i < resultPanels.Count; i++)
            {
                resultPanels[i].UpdateState(false);
            }
        }

        public void OnDeclareComplete(DeclarResponse result)
        {
            homeButton.gameObject.SetActive(false);
            _isEliminated = false;
            foreach (var resultPanel in resultPanels)
            {
                if (resultPanel.GetUserId == result.userId)
                {
                    resultPanel.UpdateState(true);
                    resultPanel.PlayerPanelReset();
                    resultPanel.SetDeclareDetail(result);
                }
            }

            UpdatePlayerPosition(result.gameResult);
            if (result.isEliminated)
            {
                //CancelInvoke(nameof(UpdateNextTimer));
                _isEliminated = true;
                remaininMatchTime = 0;
                remainingTimeText.text = "";
                LeanTween.delayedCall(3.0f, () =>
                {
                    homeButton.gameObject.SetActive(true);
                });
            }
        }

        public void UpdatePlayerPosition(List<GameResult> gameResult)
        {
            if (gameResult.Count > 0)
            {
                for (int i = 0; i < gameResult.Count; i++)
                {
                    foreach (var playerResult in resultPanels)
                    {
                        if (playerResult.GetUserId == gameResult[i].userId)
                        {
                            playerResult.UpdatePosition(gameResult[i].position);
                            break;
                        }
                    }
                }
            }
        }

        private void ResetPlayerPanel()
        {
            foreach (var resultPanel in resultPanels)
            {
                resultPanel.UpdateState(false);
            }
        }

        public void OnPlayerDeclare(DeclarResponse declarResponse)
        {

        }


        /// <summary>
        /// This is Temp Function
        /// </summary>
        public void OnHomeBtnClick()
        {
            UiManager.GetInstance.EnableMainMenuUi();
            UiManager.GetInstance.DisableGamplayScreen();
            UiManager.GetInstance.LeaveSocketRoom();
        }

        public void UpdateNextMatchTimer(int timer)
        {
            remaininMatchTime = timer;
            //CancelInvoke(nameof(UpdateNextTimer));
            if (!isTimerStarted)
            {
                isTimerStarted = true;
                if (!_isEliminated)
                {
                    UpdateNextTimer();
                }
            }
        }

        private void UpdateNextTimer()
        {
            remainingTimeText.text = "Next Match in " + remaininMatchTime + "...";
            if (remaininMatchTime > 0)
            {
                LeanTween.delayedCall(1.0f, () =>
                {
                    remaininMatchTime--;
                    if (!_isEliminated)
                    {
                        UpdateNextTimer();
                    }
                });
            }
            else
            {
                remaininMatchTime = 0;
                remainingTimeText.text = "Next Match in " + remaininMatchTime + "...";
            }
        }
    }
}