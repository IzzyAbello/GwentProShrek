using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayCardsInZone : MonoBehaviour
{
    public GameObject cardInHand;
    public GameObject playerHand;

    void Start()
    {

    }

    public void OnClick()
    {
        for (int i = 0; i < 1; i++)
        {
            GameObject playerCard = Instantiate(cardInHand, new Vector3(0,0,0), Quaternion.identity);
            playerCard.transform.SetParent(playerHand.transform, false);
        }
    }
}
