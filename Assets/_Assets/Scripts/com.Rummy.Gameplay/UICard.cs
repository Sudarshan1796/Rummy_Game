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

        internal void SetDetail(GameVariables.CardType cardValue, GameVariables.SuitType suitType)
        {
            image.sprite = CardController.GetInstance.GetSprite(cardValue, suitType);
        }
    }
}