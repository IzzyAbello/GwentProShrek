using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropZoneCards : MonoBehaviour
{
    public List<GameObject> cardsDropZone = new List<GameObject>();
    public bool isPoweredUp = false;
    public bool isUnderClimateEffect = false;

    public int pointsInDropZone;


    public void ClearEffectsDropZone ()
    {
        isPoweredUp = false;
        isUnderClimateEffect = false;

        for (int i = 0; i < cardsDropZone.Count; i++)
        {
            cardsDropZone[i].GetComponent<DisplayCard>().cardPower = cardsDropZone[i].GetComponent<DisplayCard>().cardPowerOG;
            cardsDropZone[i].GetComponent<DisplayCard>().powerText.text = cardsDropZone[i].GetComponent<DisplayCard>().cardPower.ToString();
            cardsDropZone[i].GetComponent<DisplayCard>().powerText.color = Color.black;
            cardsDropZone[i].GetComponent<DisplayCard>().displayCard.cardPower = cardsDropZone[i].GetComponent<DisplayCard>().displayCard.cardPowerOG;
            cardsDropZone[i].GetComponent<DisplayCard>().isPowerUp = false;
            cardsDropZone[i].GetComponent<DisplayCard>().isUnderClimateEffect = false;
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


    public void PowerUpDropZone ()
    {
        isPoweredUp = true;

        for (int i = 0; i < cardsDropZone.Count; i++)
        {
            if (!cardsDropZone[i].GetComponent<DisplayCard>().isPowerUp && cardsDropZone[i].GetComponent<DisplayCard>().displayCard.cardKind != 'g')
            {
                cardsDropZone[i].GetComponent<DisplayCard>().displayCard.cardPower *= 2;
                cardsDropZone[i].GetComponent<DisplayCard>().cardPower *= 2;
                cardsDropZone[i].GetComponent<DisplayCard>().powerText.text = cardsDropZone[i].GetComponent<DisplayCard>().cardPower.ToString();
                cardsDropZone[i].GetComponent<DisplayCard>().powerText.color = Color.green;
                cardsDropZone[i].GetComponent<DisplayCard>().isPowerUp = true;
            }
        }
    }


    public void ClimateEffectDropZone()
    {
        isUnderClimateEffect = true;

        for (int i = 0; i < cardsDropZone.Count; i++)
        {
            if (!cardsDropZone[i].GetComponent<DisplayCard>().isUnderClimateEffect && cardsDropZone[i].GetComponent<DisplayCard>().displayCard.cardKind != 'g')
            {
                cardsDropZone[i].GetComponent<DisplayCard>().displayCard.cardPower = 1;
                cardsDropZone[i].GetComponent<DisplayCard>().cardPower = 1;
                cardsDropZone[i].GetComponent<DisplayCard>().powerText.text = cardsDropZone[i].GetComponent<DisplayCard>().cardPower.ToString();
                cardsDropZone[i].GetComponent<DisplayCard>().powerText.color = Color.red;
                cardsDropZone[i].GetComponent<DisplayCard>().isUnderClimateEffect = true;
            }
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
