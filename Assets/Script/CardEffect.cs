using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEffect : MonoBehaviour
{
    private GameObject hand;

    private GameObject cardToRemove;

    public void PlayEffect ()
    {
        string effect = GetComponent<DisplayCard>().cardEffect;

        GameObject dropZone = gameObject.transform.parent.gameObject;
        DropZoneCards dropZoneCards = dropZone.GetComponent<DropZoneCards>();

        if (effect == "PowerUp" || dropZone.GetComponent<DropZoneCards>().isPoweredUp)
        {
            if (gameObject.GetComponent<DisplayCard>().displayCard.cardKind != 'g')
            {

                gameObject.GetComponent<DisplayCard>().displayCard.cardPower *= 2;
                gameObject.GetComponent<DisplayCard>().cardPower *= 2;
                gameObject.GetComponent<DisplayCard>().powerText.text = gameObject.GetComponent<DisplayCard>().cardPower.ToString();
                gameObject.GetComponent<DisplayCard>().powerText.color = Color.green;
                gameObject.GetComponent<DisplayCard>().isPowerUp = true;
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
            int aux = -1;

            for (int i = 0; i < dropZoneCards.cardsDropZone.Count; i++)
            {
                if (aux < dropZoneCards.cardsDropZone[i].GetComponent<DisplayCard>().cardPower && dropZoneCards.cardsDropZone[i].GetComponent<DisplayCard>().cardKind != 'g')
                {
                    cardToRemove = dropZoneCards.cardsDropZone[i];
                    aux = dropZoneCards.cardsDropZone[i].GetComponent<DisplayCard>().cardPower;
                }
            }

            hand = GameObject.Find("Hand");

            cardToRemove.GetComponent<DisplayCard>().cardPower = cardToRemove.GetComponent<DisplayCard>().cardPowerOG;
            cardToRemove.GetComponent<DisplayCard>().powerText.text = cardToRemove.GetComponent<DisplayCard>().cardPower.ToString();
            cardToRemove.GetComponent<DisplayCard>().powerText.color = Color.black;
            cardToRemove.GetComponent<DisplayCard>().displayCard.cardPower = cardToRemove.GetComponent<DisplayCard>().displayCard.cardPowerOG;
            cardToRemove.GetComponent<DisplayCard>().isPowerUp = false;
            cardToRemove.GetComponent<DisplayCard>().isUnderClimateEffect = false;

            cardToRemove.transform.SetParent(hand.transform, true);
        }

    }
}
