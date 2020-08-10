using com.Rummy.GameVariable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardController : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private List<Sprite> cardSprites;

    private const int SUIT_LENGTH = 13;

    private static CardController instance;

    public static CardController GetInstance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<CardController>();
            }
            return instance;
        }
    }

    public Sprite GetSprite(GameVariables.CardType cardValue, GameVariables.SuitType suitType)
    {
        int _index = ((int)suitType - 1) * SUIT_LENGTH + (int)cardValue - 1;
        return cardSprites[_index];
    }

    public GameObject GetObject(Transform parentTrasnform)
    {
        var _gObject = Instantiate(cardPrefab, parentTrasnform);
        return _gObject;
    }
}
