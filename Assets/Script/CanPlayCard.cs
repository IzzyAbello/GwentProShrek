using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanPlayCard : MonoBehaviour
{
    public bool CanPlaySpecificCard (GameObject dropZone)
    {
        DisplayCard card = gameObject.GetComponent<DisplayCard>();
        DropZoneConditions conditions = dropZone.GetComponent<DropZoneConditions>();
        DropZoneCards dropZoneCards = dropZone.GetComponent<DropZoneCards>();
        int count = dropZone.GetComponent<DropZoneCards>().cardsDropZone.Count;
        
        if (conditions.zone == 'C' && conditions.zone == card.cardZone)
        {
            bool canPlayClimateCard = true;

            for (int i = 0; i < count; i++)
            {
                DisplayCard thisCard = dropZoneCards.cardsDropZone[i].GetComponent<DisplayCard>();
                
                if (thisCard.cardCommunion == card.cardCommunion)
                {
                    canPlayClimateCard = false;
                }
            }
            if (canPlayClimateCard)
                return true;
            else
                return false;
        }
        if (conditions.zone == 'P' && conditions.zone == card.cardZone && conditions.faction == card.cardFaction)
        {
            if (dropZoneCards.cardsDropZone.Count != 0)
                return false;
            else
                return true;
        }
            
        if (conditions.faction == card.cardFaction && conditions.zone == card.cardZone)
            return true;
        else
            return false;
    }
}
