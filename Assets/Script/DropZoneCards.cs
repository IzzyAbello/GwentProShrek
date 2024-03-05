using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropZoneCards : MonoBehaviour
{
    public static List<GameObject> cardsDropZone = new List<GameObject>();
    
    public GameObject card;

    public static int pointsInDropZone;

    public void ClearDropZone()
    {
        foreach (Transform child in transform)
        {
                Destroy(child.gameObject);
        }
        cardsDropZone.RemoveRange(0, cardsDropZone.Count);
    }

    public static void GetPoints ()
    {
        int aux = 0;

        for (int i = 0; i < cardsDropZone.Count; i++)
        {
            DisplayCard getPower = cardsDropZone[i].GetComponent<DisplayCard>();
            aux += getPower.displayCard.cardPower;
        }
        pointsInDropZone = aux;
    }
}
