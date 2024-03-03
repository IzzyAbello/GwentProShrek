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
    public string cardName;
    public string cardDescription;
    public int cardKind;
    public int cardZone;
    public int cardEffect;
    public Sprite cardSprite;

    public Card ()
    {

    }

    public Card (int cardId, int cardFaction, int cardPower, string cardName, string cardDescription, int cardKind, int cardZone, int cardEffect, Sprite cardSprite)
    {
        this.cardId = cardId;
        this.cardFaction = cardFaction;
        this.cardPower = cardPower;
        this.cardName = cardName;
        this.cardDescription = cardDescription;
        this.cardKind = cardKind;
        this.cardZone = cardZone;
        this.cardEffect = cardEffect;
        this.cardSprite = cardSprite;
    }
}
