using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnClickPowerUp : MonoBehaviour
{
    public void OnCLickPowerUp()
    {
        DropZoneCards.isPowerUp = true;

        for (int i = 0; i < DropZoneCards.cardsDropZone.Count; i++)
        {
            if (!DropZoneCards.cardsDropZone[i].GetComponent<DisplayCard>().isPowerUp)
            {
                DropZoneCards.cardsDropZone[i].GetComponent<DisplayCard>().displayCard.cardPower *= 2;
                DropZoneCards.cardsDropZone[i].GetComponent<DisplayCard>().cardPower *= 2;
                DropZoneCards.cardsDropZone[i].GetComponent<DisplayCard>().powerText.text = DropZoneCards.cardsDropZone[i].GetComponent<DisplayCard>().cardPower.ToString();
                DropZoneCards.cardsDropZone[i].GetComponent<DisplayCard>().powerText.color = Color.green;
                DropZoneCards.cardsDropZone[i].GetComponent<DisplayCard>().isPowerUp = true;
            }
        }
    }
}
