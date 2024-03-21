﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEffect : MonoBehaviour
{
    private GameObject hand;
    private GameObject graveyard;
    private GameObject graveyardOpposite;
    private GameObject cardToRemove;
    private GameObject cardToRemoveOpposite;
    

    public void PlayEffect ()
    {
        string effect = GetComponent<DisplayCard>().cardEffect;

        GameObject dropZone = gameObject.transform.parent.gameObject;
        DropZoneCards dropZoneCards = dropZone.GetComponent<DropZoneCards>();

        GameObject opposite = dropZone.GetComponent<DropZoneConditions>().oppositeDropZone;
        DropZoneCards oppositeCards = opposite.GetComponent<DropZoneCards>();

        if (effect == "PowerUp" || dropZone.GetComponent<DropZoneCards>().isPoweredUp)
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

        if (effect == "Climate" || dropZone.GetComponent<DropZoneCards>().isUnderClimateEffect)
        {
            if (GetComponent<DisplayCard>().cardKind != 'g')
            {
                string climateType = GetComponent<DisplayCard>().cardCommunion;
                DropZoneCards climateZone = dropZone.GetComponent<DropZoneCards>();
                climateZone.climateS = dropZoneCards.climateS;
                climateZone.climateR = dropZoneCards.climateR;
                climateZone.climateM = dropZoneCards.climateM;

                if (effect != "Climate")
                {
                    GetComponent<DisplayCard>().CardUnderClimateEffect();
                }
                else
                {
                    if (climateType == "Rain")
                    {
                        climateZone.climateS.GetComponent<DropZoneCards>().ClimateEffectDropZone();
                        climateZone.climateS.GetComponent<DropZoneConditions>().oppositeDropZone.GetComponent<DropZoneCards>().ClimateEffectDropZone();
                    }

                    if (climateType == "Wind")
                    {
                        climateZone.climateR.GetComponent<DropZoneCards>().ClimateEffectDropZone();
                        climateZone.climateR.GetComponent<DropZoneConditions>().oppositeDropZone.GetComponent<DropZoneCards>().ClimateEffectDropZone();
                    }

                    if (climateType == "Snow")
                    {
                        climateZone.climateM.GetComponent<DropZoneCards>().ClimateEffectDropZone();
                        climateZone.climateM.GetComponent<DropZoneConditions>().oppositeDropZone.GetComponent<DropZoneCards>().ClimateEffectDropZone();
                    }
                    if (climateType == "ClearClimate") //Through GameMaster
                    {
                        GameObject clear = GameObject.Find("Clear");
                        clear.GetComponent<ClearAllField>().ClearClimate();
                    }
                }
            }
        }

        if (effect == "Decoy")
        {
            dropZoneCards.isDecoy = true;
        }

        if (effect == "Destroyer" || effect == "WeakDestroyer")
        {
            (int, GameObject) toReturnSelf;
            (int, GameObject) toReturnOpposite;

            if (effect == "Destroyer")
            {
                toReturnSelf = dropZoneCards.GetTheHighestCard();
                toReturnOpposite = oppositeCards.GetTheHighestCard();
            }
            else
            {
                toReturnSelf = dropZoneCards.GetTheLowestCard();
                toReturnOpposite = oppositeCards.GetTheLowestCard();
            }

            int cardIndexSelf = toReturnSelf.Item1;
            int cardIndexOpposite = toReturnOpposite.Item1;

            cardToRemove = toReturnSelf.Item2;
            cardToRemove.GetComponent<DisplayCard>().CardReset();

            graveyard = dropZone.GetComponent<DropZoneConditions>().graveyard;

            Debug.Log($"Card removed from Drop Zone: {cardToRemove.name}");
            Debug.Log($"Card added to Graveyard: {cardToRemove.name}");
            dropZoneCards.cardsDropZone.RemoveAt(cardIndexSelf);
            cardToRemove.transform.SetParent(graveyard.transform, true);
            Destroy(cardToRemove.gameObject);
            cardToRemove = null;

            if (toReturnOpposite.Item2 != null)
            {
                cardToRemoveOpposite = toReturnOpposite.Item2;
                cardToRemoveOpposite.GetComponent<DisplayCard>().CardReset();
                graveyardOpposite = dropZone.GetComponent<DropZoneConditions>().oppositeDropZone.GetComponent<DropZoneConditions>().graveyard;
                Debug.Log($"Card removed from Drop Zone: {cardToRemoveOpposite.name}");
                Debug.Log($"Card added to Graveyard: {cardToRemoveOpposite.name}");
                oppositeCards.cardsDropZone.RemoveAt(cardIndexOpposite);
                cardToRemoveOpposite.transform.SetParent(graveyardOpposite.transform, true);
                Destroy(cardToRemoveOpposite.gameObject);
                cardToRemoveOpposite = null;
            }     
        }

        if (effect == "Communion")
        {
            string cardId = gameObject.GetComponent<DisplayCard>().cardCommunion;

            int timesPowerUp = dropZoneCards.GetCountOfSpecifiedCard(cardId);

            if (timesPowerUp > 1)
                dropZoneCards.PowerUpSpecifiedCard(cardId, timesPowerUp);
        }

        if (effect == "Average")
        {
            dropZoneCards.AverageCardsInDropZone();
            oppositeCards.AverageCardsInDropZone();
        }

        if (effect == "Take")
        {
            if (GetComponent<DisplayCard>().cardFaction == 0)
                hand = GameObject.Find("HandShrek");
            else
                hand = GameObject.Find("HandBad");

            hand.GetComponent<Hand>().OnClickTakeFromDeck();
        }

        if (effect == "DestroyLine")
        {
            dropZoneCards.ClearDropZone(true);
            oppositeCards.ClearDropZone(true);
        }
    }
}
