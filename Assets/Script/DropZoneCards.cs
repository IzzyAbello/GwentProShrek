using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropZoneCards : MonoBehaviour
{
    public static List<GameObject> cardsDropZone = new List<GameObject>();
    
    private GameObject card;

    public static int pointsInDropZone;
    public static bool isPowerUp = false;

    public void ClearDropZone()
    {
        for (int i = 0; i < cardsDropZone.Count; i++)
        {
            cardsDropZone[i].GetComponent<DisplayCard>().cardPower = cardsDropZone[i].GetComponent<DisplayCard>().cardPowerOG;
            DropZoneCards.cardsDropZone[i].GetComponent<DisplayCard>().powerText.text = DropZoneCards.cardsDropZone[i].GetComponent<DisplayCard>().cardPower.ToString();
            DropZoneCards.cardsDropZone[i].GetComponent<DisplayCard>().powerText.color = Color.black;
            DropZoneCards.cardsDropZone[i].GetComponent<DisplayCard>().displayCard.cardPower /= 2;
            DropZoneCards.cardsDropZone[i].GetComponent<DisplayCard>().isPowerUp = false;
        }

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
            aux += cardsDropZone[i].GetComponent<DisplayCard>().cardPower;
        }
        pointsInDropZone = aux;
    }
}
