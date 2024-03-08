using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEffect : MonoBehaviour
{
    private GameObject hand; // Change public when 2 players
    private GameObject graveyard;
    private GameObject cardToRemove;

    public void PlayEffect ()
    {
        string effect = GetComponent<DisplayCard>().cardEffect;

        GameObject dropZone = gameObject.transform.parent.gameObject;
        DropZoneCards dropZoneCards = dropZone.GetComponent<DropZoneCards>(); //Change to public

        if (effect == "PowerUp" || dropZone.GetComponent<DropZoneCards>().isPoweredUp) //Change for special
        {
            if (gameObject.GetComponent<DisplayCard>().displayCard.cardKind != 'g')
            {

                gameObject.GetComponent<DisplayCard>().CardPowerUp();
            }
            else
            {
                dropZoneCards.PowerUpDropZone();
            }
        }

        if (effect == "Climate" || dropZone.GetComponent<DropZoneCards>().isUnderClimateEffect) //Enemy Field Comming Soon
        {
            dropZoneCards.ClimateEffectDropZone();
        }

        if (effect == "Decoy")
        {
            var toReturn = dropZoneCards.GetTheHighestCard();
            int cardIndex = toReturn.Item1;
            cardToRemove = toReturn.Item2;

            hand = GameObject.Find("Hand");

            cardToRemove.GetComponent<DisplayCard>().CardReset();


            Debug.Log($"Card removed from Drop Zone: {cardToRemove.name}");
            Debug.Log($"Card added to Hand: {cardToRemove.name}");

            dropZoneCards.cardsDropZone.RemoveAt(cardIndex);
            cardToRemove.transform.SetParent(hand.transform, true);
            cardToRemove = null;
        }

        if (effect == "Destroyer") // Destroy in both sides
        {
            var toReturn = dropZoneCards.GetTheHighestCard();
            int cardIndex = toReturn.Item1;
            cardToRemove = toReturn.Item2;

            cardToRemove.GetComponent<DisplayCard>().CardReset();

            graveyard = GameObject.Find("GraveYard");

            Debug.Log($"Card removed from Drop Zone: {cardToRemove.name}");
            Debug.Log($"Card added to Graveyard: {cardToRemove.name}");

            dropZoneCards.cardsDropZone.RemoveAt(cardIndex);
            cardToRemove.transform.SetParent(graveyard.transform, true);
            Destroy(cardToRemove.gameObject);
            cardToRemove = null;
        }

        if (effect == "WeakDestroyer") // Destroy in both sides
        {
            var toReturn = dropZoneCards.GetTheLowestCard();
            int cardIndex = toReturn.Item1;
            cardToRemove = toReturn.Item2;

            cardToRemove.GetComponent<DisplayCard>().CardReset();

            graveyard = GameObject.Find("GraveYard");

            Debug.Log($"Card removed from Drop Zone: {cardToRemove.name}");
            Debug.Log($"Card added to Graveyard: {cardToRemove.name}");

            dropZoneCards.cardsDropZone.RemoveAt(cardIndex);
            cardToRemove.transform.SetParent(graveyard.transform, true);
            Destroy(cardToRemove.gameObject);
            cardToRemove = null;
        }

        if (effect == "Communion")
        {
            int cardId = gameObject.GetComponent<DisplayCard>().cardId;

            int timesPowerUp = dropZoneCards.GetCountOfSpecifiedCard(cardId);

            if (timesPowerUp > 1)
                dropZoneCards.PowerUpSpecifiedCard(cardId, timesPowerUp);
        }

        if (effect == "ClearClimate")
        {
            dropZoneCards.ResetAllCards();
        }
    }
}
