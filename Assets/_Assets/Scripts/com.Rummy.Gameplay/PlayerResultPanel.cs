using com.Rummy.Network;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace com.Rummy.UI
{
    public class PlayerResultPanel : MonoBehaviour
    {
        [SerializeField] private TMP_Text userName;
        [SerializeField] private TMP_Text currentScore;
        [SerializeField] private TMP_Text totalScore;
        [SerializeField] private GameObject cardGameObject;
        [SerializeField] private List<GameObject> groupObject;
        [SerializeField] private GameObject dropGameobject;
        [SerializeField] private GameObject wonGameObject;

        private int _userId;
        private List<GameObject> cardObects;
        internal int GetUserId
        {
            get
            {
                return _userId;
            }
        }
        private void Awake()
        {
            cardObects = new List<GameObject>();
        }

        private void OnDisable()
        {
            foreach (var gObject in cardObects)
            {
                Destroy(gObject);
            }
        }

        internal void PlayerPanelReset()
        {
            foreach (var group in groupObject)
            {
                group.SetActive(false);
            }
            dropGameobject.SetActive(false);
            _userId = 0;
        }

        internal void UpdateState(bool state)
        {
            gameObject.SetActive(state);
        }

        internal void SetDetails(Result result)
        {
            Debug.Log("This round Complete is player "+result.userId);
            Debug.Log("Player Card Count" + result.cardGroup.Count+":"+gameObject.name);
            _userId = result.userId;
            userName.text = result.userName;
            currentScore.text = result.points.ToString();
            totalScore.text = result.totalPoints.ToString();
            wonGameObject.SetActive(result.isWinner);
            for (int i = 0; i < result.cardGroup.Count; i++)
            {
                if (result.isDropped)
                {
                    dropGameobject.SetActive(true);
                    continue;
                }

                groupObject[i].SetActive(true);

                for (int j = 0; j < result.cardGroup[i].card_set.Count; j++)
                {
                    var gObject = Instantiate(cardGameObject);
                    gObject.transform.SetParent(groupObject[i].transform);
                    gObject.GetComponent<UICard>().SetDetail(result.cardGroup[i].card_set[j].cardValue,
                        result.cardGroup[i].card_set[j].suitValue);
                    cardObects.Add(gObject);
                }
            }
        }

        internal void SetDeclareDetail(DeclarResponse result)
        {
            Debug.Log("This declare is player " + result.userId);
            Debug.Log("Player Card Count" + result.cardGroup.Count);
            PlayerPanelReset();
            OnDisable();
            _userId = result.userId;
            userName.text = result.userName;
            currentScore.text = result.points.ToString();
            totalScore.text = result.totalPoints.ToString();
            wonGameObject.SetActive(result.isWinner);
            for (int i = 0; i < result.cardGroup.Count; i++)
            {
                //if (result.droped)
                //{
                //   dropGameobject.SetActive(true);
                //    continue;
                //}
                groupObject[i].SetActive(true);
                for (int j = 0; j < result.cardGroup[i].card_set.Count; j++)
                {
                    var gObject = Instantiate(cardGameObject);
                    gObject.transform.SetParent(groupObject[i].transform);
                    gObject.GetComponent<UICard>().SetDetail(result.cardGroup[i].card_set[j].cardValue,
                        result.cardGroup[i].card_set[j].suitValue);
                    cardObects.Add(gObject);
                }
            }

        }

    }
}