﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropZoneCards : MonoBehaviour
{
    public List<GameObject> cardsDropZone = new List<GameObject>();
    public bool isPoweredUp = false;
    public bool isUnderClimateEffect = false;
    public bool isAveraged = false;

    public int pointsInDropZone;


    public void ClearEffectsDropZone ()
    {
        isPoweredUp = false;
        isUnderClimateEffect = false;

        for (int i = 0; i < cardsDropZone.Count; i++)
        {
            cardsDropZone[i].GetComponent<DisplayCard>().CardReset();
        }
    }

    public void ResetAllCards()
    {
        for (int i = 0; i < cardsDropZone.Count; i++)
        {
            isUnderClimateEffect = false;
            DisplayCard card = cardsDropZone[i].GetComponent<DisplayCard>();
            card.CardReset();
            if (isPoweredUp)
                card.CardPowerUp();
            if (card.cardEffect == "Communion")
                CommunionActivation();
        }
    }

    public void ClearDropZone()
    {
        ClearEffectsDropZone();

        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        cardsDropZone.RemoveRange(0, cardsDropZone.Count);
    }


    public void CommunionActivation ()
    {
        int aux = 0;
        string cardId = null;
        for (int i = 0; i < cardsDropZone.Count; i++)
        {
            DisplayCard card = cardsDropZone[i].GetComponent<DisplayCard>();
            if (card.cardEffect == "Communion")
            {
                aux++;
                cardId = card.cardCommunion;
            }
        }
        if (aux > 1)
            for (int i = 0; i < cardsDropZone.Count; i++)
            {
                PowerUpSpecifiedCard(cardId, aux);
            }
    }

    public void PowerUpDropZone ()
    {
        isPoweredUp = true;

        for (int i = 0; i < cardsDropZone.Count; i++)
        {
            DisplayCard card = cardsDropZone[i].GetComponent<DisplayCard>();

            if (!card.isPowerUp && card.displayCard.cardKind != 'g')
            {
                card.CardPowerUp();
            }
        }
    }

    public void PowerUpSpecifiedCard(string id, int times)
    {
        for (int i = 0; i < cardsDropZone.Count; i++)
        {
            DisplayCard card = cardsDropZone[i].GetComponent<DisplayCard>();

            if (card.displayCard.cardKind != 'g' && card.cardCommunion == id)
            {
                card.CardReset();
                if (!isPoweredUp && !isUnderClimateEffect)
                    card.CardPowerUp(times);
                if (isPoweredUp)
                {
                    card.CardPowerUp();
                    card.CardPowerUp(times);
                }
                if (isUnderClimateEffect)
                {
                    card.CardUnderClimateEffect();
                }
            }
        }
    }

    public void ClimateEffectDropZone()
    {
        isUnderClimateEffect = true;

        for (int i = 0; i < cardsDropZone.Count; i++)
        {
            DisplayCard card = cardsDropZone[i].GetComponent<DisplayCard>();

            if (!card.isUnderClimateEffect && card.displayCard.cardKind != 'g')
            {
                cardsDropZone[i].GetComponent<DisplayCard>().CardUnderClimateEffect();
            }
        }
    }

    public (int ,GameObject) GetTheHighestCard()
    {
        int aux = -1;
        int cardIndex = 0;
        GameObject getCard = null;

        for (int i = 0; i < cardsDropZone.Count; i++)
        {
            DisplayCard card = cardsDropZone[i].GetComponent<DisplayCard>();

            if (aux < card.cardPower && card.cardKind != 'g')
            {
                getCard = cardsDropZone[i];
                aux = cardsDropZone[i].GetComponent<DisplayCard>().cardPower;
                cardIndex = i;
            }
        }

        var ans = (cardIndex, getCard);

        return ans;
    }

    public (int, GameObject) GetTheLowestCard()
    {
        int aux = 100;
        int cardIndex = 0;
        GameObject getCard = null;

        for (int i = 0; i < cardsDropZone.Count; i++)
        {
            DisplayCard card = cardsDropZone[i].GetComponent<DisplayCard>();

            if (aux > card.cardPower && card.cardKind != 'g')
            {
                getCard = cardsDropZone[i];
                aux = cardsDropZone[i].GetComponent<DisplayCard>().cardPower;
                cardIndex = i;
            }
        }

        var ans = (cardIndex, getCard);

        return ans;
    }

    public int GetCountOfSpecifiedCard (string id)
    {
        int ans = 0;
        for (int i = 0; i < cardsDropZone.Count; i++)
        {
            if (cardsDropZone[i].GetComponent<DisplayCard>().cardCommunion == id)
                ans++;
        }
        return ans;
    }

    public void AverageCardsInDropZone()
    {
        int average = 0;
        for (int i = 0; i < cardsDropZone.Count; i++)
        {
            average += cardsDropZone[i].GetComponent<DisplayCard>().cardPower;
        }
        average /= cardsDropZone.Count;

        for (int i = 0; i < cardsDropZone.Count; i++)
        {
            DisplayCard card = cardsDropZone[i].GetComponent<DisplayCard>();
            if (card.cardKind != 'g')
                card.AverageCard(average);

        }
    }

    public void GetPoints ()
    {
        int aux = 0;

        for (int i = 0; i < cardsDropZone.Count; i++)
        {
            aux += cardsDropZone[i].GetComponent<DisplayCard>().cardPower;
        }
        pointsInDropZone = aux;
    }
}
