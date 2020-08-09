using com.Rummy.GameVariable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardProperties", menuName = "ScriptableObjects/CardProperties")]
public class CardScriptableObject : ScriptableObject
{
    public GameVariables.CardType cardValue;
    public GameVariables.SuitType suitType;
}
