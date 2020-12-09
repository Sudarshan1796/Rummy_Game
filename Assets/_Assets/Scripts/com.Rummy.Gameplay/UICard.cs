using com.Rummy.Gameplay;
using com.Rummy.GameVariable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace com.Rummy.UI
{
    public class UICard : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private GameObject wildCardJ;

        internal void SetDetail(GameVariables.CardType cardValue, GameVariables.SuitType suitType)
        {
            image.sprite = CardController.GetInstance.GetSprite(cardValue, suitType);
            wildCardJ.SetActive(GamePlayManager.GetInstance.wildCard.suitValue == suitType &&
                                GamePlayManager.GetInstance.wildCard.cardValue == cardValue);
        }
    }
}