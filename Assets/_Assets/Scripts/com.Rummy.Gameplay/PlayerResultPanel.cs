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

        internal void PlayerPanelReset()
        {
            foreach (var group in groupObject)
            {
                group.SetActive(false);
            }
            dropGameobject.SetActive(false);
        }

        internal void UpdateState(bool state)
        {
            gameObject.SetActive(state);
        }

        internal void SetDetails(Result result)
        {
            for (int i = 0; i < result.cardGroup.Count; i++)
            {
                if (result.isDropped)
                {
                    dropGameobject.SetActive(true);
                    continue;
                }
                groupObject[i].SetActive(true);

                for (int j = 0; j < result.cardGroup[i].card_set.Count; i++)
                {
                    var gObject = Instantiate(cardGameObject);
                    gObject.transform.SetParent(groupObject[i].transform);
                    gObject.GetComponent<UICard>().SetDetail(result.cardGroup[i].card_set[i].cardValue, result.cardGroup[i].card_set[i].suitValue);
                }
            }
        }
    }
}