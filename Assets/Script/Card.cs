using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]


[CreateAssetMenu(fileName = "New Card", menuName = "Card", order = 1)]
public class Card : ScriptableObject
{
    public int cardId;
    public int cardFaction;
    public int cardPower;
    public int cardPowerOG;
    public string cardName;
    public string cardDescription;
    public char cardKind = 's';               // s, g
    public char cardZone = 'M';               // M, R, S
    public string cardEffect = "None";
    public Sprite cardSprite;

    public Card ()
    {

    }
}
