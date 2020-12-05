using com.Rummy.Network;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.Rummy.UI
{
    public class ResultScreen : MonoBehaviour
    {
        [SerializeField] private List<PlayerResultPanel> resultPanels;

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
            ResetPlayerPanel();
            for (int i = 0; i < response.result.Count; i++)
            {
                resultPanels[i].UpdateState(true);
                resultPanels[i].PlayerPanelReset();
                resultPanels[i].SetDetails(response.result[i]);
            }
        }

        private void ResetPlayerPanel()
        {
            foreach (var resultPanel in resultPanels)
            {
                resultPanel.UpdateState(false);
            }
        }
    }
}