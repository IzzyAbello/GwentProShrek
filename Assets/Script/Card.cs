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
    public string cardDescription = "";
    public string cardCommunion = "None";     // Pork0
    public char cardKind = 's';               // s, g
    public char cardZone = 'M';               // M, R, S
    public string cardEffect = "None";
    public Sprite cardSprite;

    public Card(int cardId, int cardFaction, int cardPowerOG, string cardName, char cardKind, char cardZone)
    {
        this.cardId = cardId;
        this.cardFaction = cardFaction;
        this.cardPowerOG = cardPowerOG;
        cardPower = cardPowerOG;
        this.cardName = cardName;
        this.cardKind = cardKind;
        this.cardZone = cardZone;
        this.cardEffect = "Special";
    }

    public Card ()
    {

    }
}
