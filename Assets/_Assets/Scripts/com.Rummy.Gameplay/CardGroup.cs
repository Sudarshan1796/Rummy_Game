using com.Rummy.Gameplay;
using com.Rummy.GameVariable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardGroup : MonoBehaviour
{
    private List<Card> cards;
    private List<GameObject> cardGameobject;

    private void ChangeChildIndex(int Index)
    {

    }
    internal void Init(List<Card> cards, List<GameObject> cardObjects, GameObject parentObject, GameObject tempParent)
    {
        this.cards = new List<Card>();
        this.cards = cards;
    }

    internal void AddCardToGroup(Card Card,GameObject cardObject)
    {

    }
    internal void RemoveCardFromGroup(Card Card, GameObject cardObject)
    {

    }
}
