using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardDatabase : MonoBehaviour
{
    public static List<Card> cardListDatabase = new List<Card>();

    Card globalCard;
    
        
    //id, faction, power, name, description, kind, zone, effect, sprite

    void Awake ()
    {
        Debug.Log("Creating Card Database...\n");

        /*(0, 0, 1, "Name", "Description", 0, 0, 0, Resources.Load<Sprite>("Shrek"))*/


        //Card Template
        globalCard = ScriptableObject.CreateInstance("Card") as Card;
        globalCard.CardInit(0, 0, 1, "Name", "Description", 0, 0, 0, Resources.Load<Sprite>("Shrek"));
        cardListDatabase.Add(globalCard);


        //Good Faction
        globalCard = ScriptableObject.CreateInstance("Card") as Card;
        globalCard.CardInit(1, 0, 15, "Shrek", "Mejor fuera que dentro...", 2, 0, 0, Resources.Load<Sprite>("Shrek"));
        cardListDatabase.Add(globalCard);

        globalCard = ScriptableObject.CreateInstance("Card") as Card;
        globalCard.CardInit(2, 0, 3, "Burro", "el burro", 0, 0, 0, Resources.Load<Sprite>("Burro"));
        cardListDatabase.Add(globalCard);

        //Bad Faction
        globalCard = ScriptableObject.CreateInstance("Card") as Card;
        globalCard.CardInit(3, 1, 15, "Lord Farquad", "el lord farquad", 2, 2, 0, Resources.Load<Sprite>("LordFarquaad"));
        cardListDatabase.Add(globalCard);

        globalCard = ScriptableObject.CreateInstance("Card") as Card;
        globalCard.CardInit(4, 1, 3, "Rumpelstinskin", "Rumpelstinskin el malo", 0, 0, 0, Resources.Load<Sprite>("Rumpelstinskin"));
        cardListDatabase.Add(globalCard);

        Debug.Log($"Database size: {cardListDatabase.Count - 1}");
    }
}
