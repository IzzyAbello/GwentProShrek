using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardDatabase : MonoBehaviour
{
    public static List<Card> cardListDatabase = new List<Card>();

    //id, faction, power, name, description, kind, zone, effect, sprite

    void Awake ()
    {
        cardListDatabase.Add(new Card(0, 0, 1, "Name", "Description", 0, 0, 0, Resources.Load<Sprite>("Shrek"))); //Card Template

        //Good Team
        cardListDatabase.Add(new Card(1 , 0, 15, "Shrek", "el shrek", 2, 0, 0, Resources.Load<Sprite>("Shrek")));
        cardListDatabase.Add(new Card(2, 0, 3, "Burro", "el burro", 0, 0, 0, Resources.Load<Sprite>("Burro")));

        //Bad Team
        cardListDatabase.Add(new Card(3, 1, 15, "Lord Farquad", "el lord farquad", 2, 2, 0, Resources.Load<Sprite>("LordFarquaad")));
        cardListDatabase.Add(new Card(4, 1, 3, "Rumpelstinskin", "Rumpelstinskin el malo", 0, 0, 0, Resources.Load<Sprite>("Rumpelstinskin")));
    }
}
