using com.Rummy.Gameplay;
using com.Rummy.GameVariable;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.Rummy.UI
{
    public class GameplayController : MonoBehaviour
    {
        [SerializeField] private GameObject[] cardPosition;
        [SerializeField] private GameObject cardInitPosition;
        [SerializeField] private GameVariables.CardType[] tempcardValue;
        [SerializeField] private GameVariables.SuitType[] tempSuitTypes;
      
        private List<Card> cards;
        private List<GameObject> cardgameObject;

        private void Awake()
        {
            cards = new List<Card>();
            cardgameObject = new List<GameObject>();
            createCard();
        }

        /// <summary>
        /// this is Temp card Creation script
        /// </summary>
        private void createCard()
        {
            for (int i = 0; i < 13; i++)
            {
                var _gameObject = CardController.GetInstance.GetObject(cardInitPosition.transform);
                var _card = _gameObject.GetComponent<Card>();
                _card.Init(i, tempcardValue[i], tempSuitTypes[i]);
                cards.Add(_card);
                cardgameObject.Add(_gameObject);
            }
            StartCoroutine(PlayCardAnimation(()=>
            {
                CardGroupController.GetInstance.InitilizeGroup(cardgameObject, cards);
            }));
        }

        /// <summary>
        /// this is the Starting Card draw animation
        /// </summary>
        /// <param name="isDone"></param>
        /// <returns></returns>
        private IEnumerator PlayCardAnimation(Action isDone)
        {
            int i = 0;
            while (i < cards.Count)
            {
                cards[i].Move(cardPosition[i].transform.position);
                i++;
                yield return new WaitForSeconds(0.10f);
            }
            yield return new WaitForSeconds(0.50f);
            isDone.Invoke();
        }
    }
}