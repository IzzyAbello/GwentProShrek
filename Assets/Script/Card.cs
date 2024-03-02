using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]

public class Card
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

    public Card (int id, int faction, int power, string name, string description, int kind, int zone, int effect, Sprite sprite)
    {
        cardId = id;
        cardFaction = faction;
        cardPower = power;
        cardName = name;
        cardDescription = description;
        cardKind = kind;
        cardZone = zone;
        cardEffect = effect;
        cardSprite = sprite;





        /** RESINGONAUTAPINGAAAAAAAAAdfjfdnddjdfnjdfjdfnjddbdhdfhdfhdvd */
    }
}
