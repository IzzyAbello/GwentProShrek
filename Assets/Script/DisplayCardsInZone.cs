using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayCardsInZone : MonoBehaviour
{
    List<GameObject> hand = new List<GameObject>();

    public GameObject cardInHand;
    public GameObject playerHand;

    void Start()
    {
        for (int i = 0; i < 2; i++)
        {
            hand.Add(Deck.deck[i]);
            Debug.Log($"Card added: {hand[i].name}\n");
        }


        for (int i = 0; i < 2; i++)
        {
            Deck.deck.RemoveAt(0);
        }

    }

    public void OnClick()
    {
        for (int i = 0; i < 2; i++)
        {
            GameObject playerCard = Instantiate(hand[i], new Vector3(0, 0, 0), Quaternion.identity);
            playerCard.transform.SetParent(playerHand.transform, false);
        }
    }
}
