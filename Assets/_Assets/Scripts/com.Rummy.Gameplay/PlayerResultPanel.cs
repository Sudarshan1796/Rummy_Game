using System.Collections;
using com.Rummy.Network;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using  UnityEngine.UI;
using Debug = UnityEngine.Debug;

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

        [SerializeField] private Image positionImage;
        [SerializeField] private Sprite[] positionSprite;
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
            _userId = result.userId;
            userName.text = result.userName;
            currentScore.text = result.points.ToString();
            totalScore.text = result.totalPoints.ToString();
            wonGameObject.SetActive(result.isWinner);
            positionImage.gameObject.SetActive(false);
            if (result.isDropped)
            {
                dropGameobject.SetActive(true);
                return;
            }
            for (int i = 0; i < result.cardGroup.Count; i++)
            {
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
            PlayerPanelReset();
            OnDisable();
            _userId = result.userId;
            userName.text = result.userName;
            currentScore.text = result.points.ToString();
            totalScore.text = result.totalPoints.ToString();
            wonGameObject.SetActive(result.isWinner);
            positionImage.gameObject.SetActive(false);
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

        public void UpdatePosition(int position)
        {
            if (positionSprite.Length>=position)
            {
                positionImage.sprite = positionSprite[position - 1];
                positionImage.gameObject.SetActive(true);
            }
            else
            {
                positionImage.gameObject.SetActive(false);
            }
        }

    }
}